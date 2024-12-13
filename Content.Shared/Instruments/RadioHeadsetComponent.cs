using Robust.Shared.GameStates;

namespace Content.Shared.Instruments;

[RegisterComponent, NetworkedComponent]
public partial class RadioHeadsetComponent : Component
{
    [ViewVariables]
    public EntityUid? Wearer;
}
