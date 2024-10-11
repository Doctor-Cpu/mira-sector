using Content.Shared.Interaction;
using Content.Shared.Whitelist;
using Robust.Shared.Map;
using Robust.Shared.Physics.Systems;
using System.Numerics;

namespace Content.Shared.MoveToEntity;

public sealed class MoveToEntitySystem : EntitySystem
{
    [Dependency] private readonly EntityLookupSystem _entityLookup = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelist = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MoveToEntityComponent, AfterInteractEvent>(OnAfterInteract);
    }

    private void OnAfterInteract(EntityUid uid, MoveToEntityComponent component, ref AfterInteractEvent args)
    {
        if (!args.CanReach ||  args.Handled)
            return;

        MoveInRange(uid, component, args.ClickLocation);
        args.Handled = true;
    }

    private void MoveInRange(EntityUid uid, MoveToEntityComponent component, EntityCoordinates coords)
    {
        var entities = _entityLookup.GetEntitiesInRange(coords, component.Radius, LookupFlags.Dynamic | LookupFlags.Sundries);

        if (entities.Count < 0)
            return;

        var uidPos = _transform.GetMapCoordinates(uid);

        foreach (var ent in entities)
        {
            if (_whitelist.IsWhitelistFailOrNull(component.Whitelist, ent))
                continue;

            var entPos = _transform.GetMapCoordinates(ent);

            if (entPos.MapId != uidPos.MapId)
                continue;

            var dir = Vector2.Subtract(entPos.Position, uidPos.Position).Normalized();

            if (dir.Length() != component.Offset)
            {
                _physics.SetLinearVelocity(ent, Vector2.Zero);
                var newPos = Vector2.Add(uidPos.Position, dir.Normalized() * component.Offset);

                var impulse = Vector2.Subtract(newPos, entPos.Position);
                impulse *= component.Scale;
                _physics.ApplyLinearImpulse(ent, impulse);
            }
        }
    }
}
