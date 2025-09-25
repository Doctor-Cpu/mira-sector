using Content.Shared.Psionics.Components;
using Content.Shared.Psionics.Events;
using Content.Shared.Psionics.Ui;
using Content.Shared.Verbs;
using JetBrains.Annotations;
using System.Linq;

namespace Content.Shared.Psionics;

public sealed partial class PsionicsSystem : EntitySystem
{
    private void InitializeGenerator()
    {
        SubscribeLocalEvent<PsionicGeneratorComponent, GetPsionicGeneratorDisiplinesEvent>(OnGeneratorGetDisiplines);
        SubscribeLocalEvent<PsionicGeneratorSelectorComponent, GetVerbsEvent<AlternativeVerb>>(OnSelectorGetVerbs);
    }

    private void OnGeneratorGetDisiplines(Entity<PsionicGeneratorComponent> ent, ref GetPsionicGeneratorDisiplinesEvent args)
    {
        args.CurrentDisipline = ent.Comp.SelectedDisipline;
        if (ent.Comp.SelectedDisipline != null)
            args.Disiplines.Add(ent.Comp.SelectedDisipline.Value);
    }

    private void OnSelectorGetVerbs(Entity<PsionicGeneratorSelectorComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract)
            return;

        var verb = new AlternativeVerb()
        {
            Text = Loc.GetString("psionic-generator-selector-verb"),
            Act = () => ShowSelectorUi(ent),
        };

        args.Verbs.Add(verb);
    }

    private void ShowSelectorUi(Entity<PsionicGeneratorSelectorComponent> ent)
    {
        var ev = new GetPsionicGeneratorDisiplinesEvent();
        RaiseLocalEvent(ent.Owner, ref ev);

        if (!ev.Disiplines.Any())
            return;

        _ui.OpenUi(ent.Owner, PsionicGeneratorSelectorUiKey.Key, true);
        var state = new PsionicGeneratorSelectorBoundUserInterfaceState(ev.CurrentDisipline, ev.Disiplines);
        _ui.SetUiState(ent.Owner, PsionicGeneratorSelectorUiKey.Key, state);
    }

    [PublicAPI]
    public bool TryDoGeneration(Entity<PsionicGeneratorComponent?> ent, EntityUid? user)
    {
        if (!Resolve(ent.Owner, ref ent.Comp))
            return false;

        if (ent.Comp.SelectedDisipline is not { } selectedDisiplineId)
            return false;

        if (ent.Comp.NextGeneration > _timing.CurTime)
            return false;

        ent.Comp.NextGeneration = _timing.CurTime + ent.Comp.GenerationDelay;

        var selectedDisipline = _prototype.Index(selectedDisiplineId);
        foreach (var effect in selectedDisipline.Effects)
        {
            if (!effect.CanDoEffect(EntityManager, ent.Owner, user, out var reason))
            {
                _popup.PopupPredicted(reason, ent.Owner, user);
                Dirty(ent);
                return false;
            }
        }

        // all doable
        // now execute them
        foreach (var effect in selectedDisipline.Effects)
            effect.DoEffect(EntityManager, ent.Owner, user);

        Dirty(ent);
        return true;
    }
}
