using Robust.Shared.Serialization;

namespace Content.Shared.Psionics.Requirements;

[ImplicitDataDefinitionForInheritors]
[Serializable, NetSerializable]
public abstract partial class BasePsionicRequirement
{
    public abstract bool RequirementMet(IEntityManager entity, EntityUid terminal, EntityUid user);
}
