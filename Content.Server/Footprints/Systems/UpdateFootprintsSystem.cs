using Content.Server.Footprints.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Components.SolutionManager;
using Robust.Shared.Physics.Events;
using Robust.Shared.Random;

namespace Content.Server.Footprint.Systems;

public sealed partial class FootprintSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<GivesFootprintsComponent, StartCollideEvent>(OnStartStep);
        SubscribeLocalEvent<GivesFootprintsComponent, EndCollideEvent>(OnEndStep);
        SubscribeLocalEvent<CanLeaveFootprintsComponent, ComponentInit>(FootprintRandom);
    }

    private void FootprintRandom(EntityUid uid, CanLeaveFootprintsComponent component, ComponentInit args)
    {
        if (!TryComp<LeavesFootprintsComponent>(uid, out var footprintComp))
        {
            RemComp<CanLeaveFootprintsComponent>(uid);
            return;
        }

        if (footprintComp.FootprintPrototypeAlternative != null)
            component.UseAlternative = _random.Prob(0.5f);
    }

    private void OnStartStep(EntityUid uid, GivesFootprintsComponent component , ref StartCollideEvent args)
    {
        if (component.Container == null ||
        !CanLeaveFootprints(args.OtherEntity, out var messMaker, uid) ||
        !TryComp<LeavesFootprintsComponent>(messMaker, out var footprintComp) ||
        !TryComp<SolutionContainerManagerComponent>(uid, out var solutionManComp) ||
        solutionManComp.Containers.Count <=0)
            return;

        if (!GetSolution(uid, solutionManComp, component.Container, out var puddleSolution) ||
        !TryComp<SolutionComponent>(puddleSolution, out var puddleSolutionComp))
            return;

        var playerFootprintComp = EnsureComp<CanLeaveFootprintsComponent>(messMaker);

        var footprintTotalUnits = footprintComp.MaxFootsteps * UnitsPerFootstep;

        if (!_solutionContainer.EnsureSolutionEntity(messMaker, component.Container, out var newSolution, footprintTotalUnits) ||
        newSolution == null)
        {
            RemComp<CanLeaveFootprintsComponent>(messMaker);
            return;
        }

        playerFootprintComp.Solution = newSolution.Value;

        var split = _solutionContainer.SplitSolution(puddleSolution.Value, footprintTotalUnits);
        _solutionContainer.TryAddSolution(playerFootprintComp.Solution, split);
        playerFootprintComp.Solution.Comp.Solution.CanReact = false;

        playerFootprintComp.LastFootstep = _transform.GetMapCoordinates(args.OtherEntity);
        playerFootprintComp.FootstepsLeft = (uint) Math.Floor(((float) playerFootprintComp.Solution.Comp.Solution.Volume / UnitsPerFootstep));
        playerFootprintComp.Container = component.Container;
        playerFootprintComp.LastPuddle = uid;
    }

    private void OnEndStep(EntityUid uid, GivesFootprintsComponent component , ref EndCollideEvent args)
    {

    }

    private bool GetSolution(EntityUid uid, SolutionContainerManagerComponent solutionManComp, string container, out Entity<SolutionComponent>? targetSolutionComp)
    {
        foreach (var solutionComp in _solutionContainer.EnumerateSolutions((uid, solutionManComp)))
        {
            if (solutionComp.Name != container)
                continue;

            targetSolutionComp = solutionComp.Solution;
            return true;
        }

        targetSolutionComp = null;
        return false;
    }
}
