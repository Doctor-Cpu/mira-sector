using Content.Shared.Inventory.Events;
using Content.Shared.Psionics.Components;
using Content.Shared.Psionics.Events;
using Content.Shared.Psionics.Ui;
using Content.Shared.Verbs;
using JetBrains.Annotations;
using System.Diagnostics.CodeAnalysis;

namespace Content.Shared.Psionics;

public sealed partial class PsionicsSystem : EntitySystem
{
    private void InitializeGenerator()
    {
        SubscribeLocalEvent<PsionicGeneratorSelectorComponent, GetVerbsEvent<AlternativeVerb>>(OnGeneratorSelectorGetVerbs);

        SubscribeLocalEvent<PsionicGeneratorClothingComponent, GotEquippedEvent>(OnGeneratorClothingEquipped);
        SubscribeLocalEvent<PsionicGeneratorClothingComponent, GotUnequippedEvent>(OnGeneratorClothingUnequipped);
        SubscribeLocalEvent<PsionicGeneratorClothingComponent, GetPsionicGeneratorUserEvent>(OnGeneratorClothingGetUser);
    }

    private void OnGeneratorSelectorGetVerbs(Entity<PsionicGeneratorSelectorComponent> ent, ref GetVerbsEvent<AlternativeVerb> args)
    {
        if (!args.CanAccess || !args.CanInteract || !args.CanComplexInteract)
            return;

        if (!TryComp<PsionicGeneratorComponent>(ent.Owner, out var generator))
            return;

        var verb = new AlternativeVerb()
        {
            Text = Loc.GetString("psionic-generator-selector-verb"),
            Act = () => ShowSelectorUi((ent.Owner, ent.Comp, generator)),
        };

        args.Verbs.Add(verb);
    }

    private void OnGeneratorClothingEquipped(Entity<PsionicGeneratorClothingComponent> ent, ref GotEquippedEvent args)
    {
        ent.Comp.Wearer = args.Equipee;
        Dirty(ent);
    }

    private void OnGeneratorClothingUnequipped(Entity<PsionicGeneratorClothingComponent> ent, ref GotUnequippedEvent args)
    {
        ent.Comp.Wearer = null;
        Dirty(ent);
    }

    private void OnGeneratorClothingGetUser(Entity<PsionicGeneratorClothingComponent> ent, ref GetPsionicGeneratorUserEvent args)
    {
        args.User ??= ent.Comp.Wearer;
    }

    private void ShowSelectorUi(Entity<PsionicGeneratorSelectorComponent, PsionicGeneratorComponent> ent)
    {
        if (!TryGetGeneratorUser(ent.Owner, out var user))
            return;

        if (!TryGetUserLearnedDisiplines(user.Value, out var disiplines))
            return;

        _ui.OpenUi(ent.Owner, PsionicGeneratorSelectorUiKey.Key, true);
        var state = new PsionicGeneratorSelectorBoundUserInterfaceState(ent.Comp2.SelectedDisipline, disiplines);
        _ui.SetUiState(ent.Owner, PsionicGeneratorSelectorUiKey.Key, state);
    }

    [PublicAPI]
    public bool TryGetGeneratorUser(EntityUid uid, [NotNullWhen(true)] out EntityUid? user)
    {
        var ev = new GetPsionicGeneratorUserEvent();
        RaiseLocalEvent(uid, ref ev);
        user = ev.User;
        return user != null;
    }

    [PublicAPI]
    public bool TryDoGeneration(Entity<PsionicGeneratorComponent?> ent, EntityUid user)
    {
        if (!Resolve(ent.Owner, ref ent.Comp))
            return false;

        if (ent.Comp.SelectedDisipline is not { } selectedDisiplineId)
            return false;

        if (ent.Comp.NextGeneration > _timing.CurTime)
            return false;

        if (!TryGetUserLearnedDisiplines(user, out var disiplines))
            return false;

        if (!disiplines.Contains(selectedDisiplineId))
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
