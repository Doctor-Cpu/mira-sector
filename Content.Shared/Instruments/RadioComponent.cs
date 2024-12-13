using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Instruments;

[RegisterComponent, NetworkedComponent]
public partial class RadioComponent : Component
{
    [ViewVariables]
    public EntityUid? Host;
}

[Serializable, NetSerializable]
public sealed class RadioHostChangeEvent : EntityEventArgs
{
    public NetEntity Uid { get; }

    public RadioHostChangeEvent(NetEntity uid)
    {
        Uid = uid;
    }
}
