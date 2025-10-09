using Content.Shared.Psionics.Components;
using Content.Shared.Psionics.Ui;
using Robust.Shared.Prototypes;
using System.Linq;

namespace Content.Shared.Psionics;

public sealed partial class PsionicsSystem : EntitySystem
{
    private void InitializeTerminal()
    {
        Subs.BuiEvents<PsionicTerminalComponent>(PsionicTerminalUiKey.Key, subs =>
        {
            subs.Event<BoundUIOpenedEvent>(OnTerminalUiOpen);
        });
    }

    private void OnTerminalUiOpen(Entity<PsionicTerminalComponent> ent, ref BoundUIOpenedEvent args)
    {
        var available = GetTerminalAvailableDisiplines(ent, args.Actor).ToHashSet();
        var state = new PsionicTerminalBoundUserInterfaceState(available, new());
        _ui.SetUiState(ent.Owner, args.UiKey, state);
    }

    private IEnumerable<ProtoId<PsionicDisiplinePrototype>> GetTerminalAvailableDisiplines(Entity<PsionicTerminalComponent> ent, EntityUid user)
    {
        foreach (var (id, proto) in _disiplines)
        {
            var reqPassed = true;
            foreach (var requirement in proto.Requirements)
            {
                if (!requirement.RequirementMet(EntityManager, ent.Owner, user))
                {
                    reqPassed = false;
                    break;
                }
            }

            if (reqPassed)
                yield return id;
        }
    }
}
