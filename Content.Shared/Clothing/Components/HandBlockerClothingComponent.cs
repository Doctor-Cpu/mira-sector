using Content.Shared.Hands.Components;
using Robust.Shared.GameStates;

namespace Content.Shared.Clothing.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class HandBlockerClothingComponent : Component
{
    [DataField]
    public Dictionary<HandLocation, int> RequiredHandsLocation = [];

    [DataField]
    public int RequiredHands;

    [DataField, AutoNetworkedField]
    public EntityUid? Wearer;

    [DataField, AutoNetworkedField]
    public string? Slot;
}
