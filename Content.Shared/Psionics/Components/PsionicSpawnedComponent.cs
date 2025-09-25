using Content.Shared.Psionics.Effects;
using Robust.Shared.GameStates;

namespace Content.Shared.Psionics.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(PsionicsSystem), typeof(BasePsionicEffect))]
public sealed partial class PsionicSpawnedComponent : Component
{
    [DataField, AutoNetworkedField]
    public EntityUid Generator;

    [DataField, AutoNetworkedField]
    public EntityUid? User;
}
