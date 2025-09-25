using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared.Psionics.Components;

[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState, AutoGenerateComponentPause]
[Access(typeof(PsionicsSystem))]
public sealed partial class PsionicGeneratorComponent : Component
{
    [DataField]
    public TimeSpan GenerationDelay = TimeSpan.FromSeconds(2);

    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer)), AutoNetworkedField, AutoPausedField]
    public TimeSpan NextGeneration;

    [DataField, AutoNetworkedField]
    public ProtoId<PsionicDisiplinePrototype>? SelectedDisipline;
}
