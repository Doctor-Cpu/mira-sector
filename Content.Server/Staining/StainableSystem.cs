using Content.Server.Body.Components;
using Content.Shared.Inventory;
using Content.Shared.Stainable;
using Content.Shared.Weapons.Melee.Events;

namespace Content.Server.Stainable;

public sealed partial class StainableSystem : SharedStainableSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StainableComponent, InventoryRelayedEvent<AttackingEvent>>((u, c, e) => OnAttack(u, c, e.Args));
    }

    private void OnAttack(EntityUid uid, StainableComponent component, AttackingEvent args)
    {
        if (!TryComp<BloodstreamComponent>(args.Target, out var bloodstream))
            return;

        if (!_solutionContainer.ResolveSolution(args.Target, bloodstream.BloodSolutionName, ref bloodstream.BloodSolution, out var _))
            return;

        SetStainColor(uid, component, bloodstream.BloodSolution.Value);
    }
}
