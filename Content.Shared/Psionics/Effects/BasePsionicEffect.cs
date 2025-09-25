using Robust.Shared.Serialization;

namespace Content.Shared.Psionics.Effects;

[ImplicitDataDefinitionForInheritors]
[Serializable, NetSerializable]
public abstract partial class BasePsionicEffect
{
    public virtual bool CanDoEffect(IEntityManager entity, EntityUid generator, EntityUid? user, out string? reason)
    {
        reason = null;
        return true;
    }

    public abstract void DoEffect(IEntityManager entity, EntityUid generator, EntityUid? user);
}
