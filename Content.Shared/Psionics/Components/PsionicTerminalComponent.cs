using Robust.Shared.GameStates;

namespace Content.Shared.Psionics.Components;

[RegisterComponent, NetworkedComponent]
[Access(typeof(PsionicsSystem))]
public sealed partial class PsionicTerminalComponent : Component;
