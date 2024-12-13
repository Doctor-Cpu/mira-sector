using Robust.Shared.Prototypes;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments;

[RegisterComponent, NetworkedComponent]
public partial class HeadphonesComponent : Component
{
    [DataField("action")]
    public EntProtoId ActionId = "ActionInstrument";

    [ViewVariables]
    public EntityUid? Action;

    [ViewVariables]
    public bool IsWorn = false;
}

[Serializable, NetSerializable]
public sealed class HeadphonesGetPlayerEvent : EntityEventArgs
{
    public NetEntity Player;

    public HeadphonesGetPlayerEvent(NetEntity player)
    {
        Player = player;
    }
}
