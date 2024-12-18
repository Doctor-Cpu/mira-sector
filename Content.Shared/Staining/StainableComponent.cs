using Content.Shared.Chemistry.Components;
using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Stainable;

[RegisterComponent, NetworkedComponent]
public sealed partial class StainableComponent : Component
{
    [DataField(required: true)]
    public SpriteSpecifier? Icon;

    [DataField]
    public string SolutionName = "stain";

    [DataField]
    public FixedPoint2 Volume = 10;

    [DataField]
    public FixedPoint2 Absorb = 10;

    [ViewVariables]
    public Entity<SolutionComponent>? Solution;
}

[Serializable, NetSerializable]
public enum StainVisuals : byte
{
    Enabled,
    Color
}

[Serializable, NetSerializable]
public enum StainLayer : byte
{
    Overlay
}
