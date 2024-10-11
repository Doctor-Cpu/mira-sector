using Content.Shared.Whitelist;
using Robust.Shared.GameStates;
using Robust.Shared.Map;

namespace Content.Shared.MoveToEntity;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class MoveToEntityComponent : Component
{
    [DataField, AutoNetworkedField]
    public bool Enabled = true;

    [DataField]
    public bool EnableWhenHeld = false;

    [DataField]
    public float Radius = 3f;

    [DataField]
    public float Scale = 25f;

    [DataField]
    public float Tolerance = 30f;

    [DataField]
    public float Offset = 1.5f;

    [ViewVariables, AutoNetworkedField]
    public MapCoordinates LastPos = MapCoordinates.Nullspace;

    [DataField(required: true)]
    public EntityWhitelist? Whitelist;
}
