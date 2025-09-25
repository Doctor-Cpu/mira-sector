using Content.Shared.Damage;
using Robust.Shared.Serialization;

namespace Content.Shared.Psionics.Effects;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class DamageUser : BasePsionicEffect
{
    [DataField(required: true)]
    public DamageSpecifier Damage;

    public override bool CanDoEffect(IEntityManager entity, EntityUid generator, EntityUid? user, out string? reason)
    {
        base.CanDoEffect(entity, generator, user, out reason);
        return user != null;
    }

    public override void DoEffect(IEntityManager entity, EntityUid generator, EntityUid? user)
    {
        var sys = entity.System<DamageableSystem>();
        sys.TryChangeDamage(user!.Value, Damage);
    }
}
