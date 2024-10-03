using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.PDA.Mesaging;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class PdaMessagingComponent : Component
{
    [ViewVariables]
    [AutoNetworkedField]
    public HashSet<PdaMessage> Messages = new();

    [ViewVariables]
    [AutoNetworkedField]
    public EntityUid? ConnectedServer;
}

[Serializable, NetSerializable]
public class PdaMessage
{
    public string RecieverName { get; set; } = String.Empty;
    public string SenerName { get; set; } = String.Empty;
    public string Message { get; set; } = String.Empty;
    public TimeSpan TimeSent { get; set; }
}
