using Robust.Shared.GameStates;

namespace Content.Shared.Psionics.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PsionicGeneratorClothingComponent : Component
{
    [ViewVariables, AutoNetworkedField]
    public EntityUid? Wearer;
}
