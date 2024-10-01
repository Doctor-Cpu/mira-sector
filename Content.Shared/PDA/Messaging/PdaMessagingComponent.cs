using Robust.Shared.GameStates;

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

public class PdaMessage
{
    public string RecieverName { get; set; } = String.Empty;
    public string SenerName { get; set; } = String.Empty;
    public string Message { get; set; } = String.Empty;
    public TimeSpan TimeSent { get; set; }
}
