using Robust.Shared.GameStates;

namespace Content.Shared.PDA.Mesaging.Server;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState]
public sealed partial class PdaMessagingServerComponent : Component
{
    [ViewVariables]
    [AutoNetworkedField]
    public Dictionary<EntityUid, string> ConnectedPdas = new();
}

[ByRefEvent]
public struct AttemptAddPdaReciever
{
    public string Name;
    public EntityUid Uid;
}
