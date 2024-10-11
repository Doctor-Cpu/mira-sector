using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Shared.MoveToEntity;

[RegisterComponent, NetworkedComponent]
public sealed partial class MoveToEntityComponent : Component
{
    [DataField]
    public float Radius = 3f;

    [DataField]
    public float Scale = 3f;

    [DataField(required: true)]
    public EntityWhitelist? Whitelist;
}
