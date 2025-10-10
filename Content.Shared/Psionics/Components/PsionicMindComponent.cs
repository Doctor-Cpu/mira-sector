using Content.Shared.Roles;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Psionics.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(PsionicsSystem))]
public sealed partial class PsionicMindComponent : BaseMindRoleComponent
{
    [DataField, AutoNetworkedField]
    public HashSet<ProtoId<PsionicDisiplinePrototype>> LearnedDisiplines = [];
}
