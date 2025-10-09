using Content.Shared.EntityTable;
using Content.Shared.EntityTable.EntitySelectors;
using Content.Shared.Psionics.Components;
using Robust.Shared.Network;
using Robust.Shared.Serialization;
using System.Linq;

namespace Content.Shared.Psionics.Effects;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class Spawn : BasePsionicEffect
{
    [DataField(required: true)]
    public EntityTableSelector Table;

    public override bool CanDoEffect(IEntityManager entity, EntityUid generator, EntityUid? user, out string? reason)
    {
        base.CanDoEffect(entity, generator, user, out reason);
        var sys = entity.System<EntityTableSystem>();
        var spawns = sys.GetSpawns(Table);

        return spawns.Any();
    }

    public override void DoEffect(IEntityManager entity, EntityUid generator, EntityUid? user)
    {
        var net = IoCManager.Resolve<INetManager>();
        var sys = entity.System<EntityTableSystem>();
        var spawns = sys.GetSpawns(Table);

        // spawning cant properly be predicted
        if (!net.IsServer)
            return;

        var xform = entity.GetComponent<TransformComponent>(generator);
        foreach (var id in spawns)
        {
            var spawned = entity.SpawnNextToOrDrop(id, generator, xform);
            var spawnedComp = new PsionicSpawnedComponent()
            {
                Generator = generator,
                User = user
            };

            entity.AddComponent(spawned, spawnedComp);
            entity.Dirty(spawned, spawnedComp);
        }

        return;
    }
}
