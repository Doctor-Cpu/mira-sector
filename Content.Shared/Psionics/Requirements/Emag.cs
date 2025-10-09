using Content.Shared.Emag.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared.Psionics.Requirements;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class Emag : BasePsionicRequirement
{
    public override bool RequirementMet(IEntityManager entity, EntityUid terminal, EntityUid user)
    {
        var sys = entity.System<EmagSystem>();
        return sys.CheckFlag(terminal, EmagType.Interaction);
    }
}
