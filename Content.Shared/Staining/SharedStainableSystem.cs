using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Fluids.Components;
using Content.Shared.Inventory;
using Content.Shared.Slippery;
using Robust.Shared.Prototypes;

namespace Content.Shared.Stainable;

public abstract partial class SharedStainableSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] protected readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] protected readonly SharedSolutionContainerSystem _solutionContainer = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<StainableComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<StainableComponent, InventoryRelayedEvent<SlippedEvent>>((u, c, e) => OnSlip(u, c, ref e.Args));
    }

    private void OnInit(EntityUid uid, StainableComponent component, ComponentInit args)
    {
        _solutionContainer.EnsureSolution(uid, component.SolutionName, out _, component.Volume);
        _appearance.SetData(uid, StainVisuals.Enabled, false);
    }

    private void OnSlip(EntityUid uid, StainableComponent component, ref SlippedEvent args)
    {
        if (!TryComp<PuddleComponent>(args.Slipper, out var puddleComp))
            return;

        if (!_solutionContainer.ResolveSolution(args.Slipper, puddleComp.SolutionName, ref puddleComp.Solution, out var _))
            return;

        SetStainColor(uid, component, puddleComp.Solution.Value);
    }

    protected void SetStainColor(EntityUid uid, StainableComponent component, Entity<SolutionComponent> solution)
    {
        if (!_solutionContainer.ResolveSolution(uid, component.SolutionName, ref component.Solution, out var _))
            return;

        var split = _solutionContainer.SplitSolution(solution, component.Absorb);
        _solutionContainer.AddSolution(component.Solution.Value, split);

        var stain = component.Solution.Value.Comp.Solution;

        var volume = stain.Volume;
        var color = stain.GetColor(_prototypeManager);

        //adjust alpha based on volume
        if (volume < component.Volume)
        {
            color.A = (float) (component.Volume / volume);
        }

        _appearance.SetData(uid, StainVisuals.Enabled, true);
        _appearance.SetData(uid, StainVisuals.Color, color);
    }
}
