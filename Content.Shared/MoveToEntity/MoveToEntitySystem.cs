using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Inventory.Events;
using Content.Shared.Whitelist;
using Robust.Shared.Map;
using Robust.Shared.Physics.Components;
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

        SubscribeLocalEvent<MoveToEntityComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<MoveToEntityComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<MoveToEntityComponent, GotUnequippedEvent>(OnUnequipped);
        SubscribeLocalEvent<MoveToEntityComponent, GotEquippedHandEvent>(OnHandEquipped);
        SubscribeLocalEvent<MoveToEntityComponent, GotUnequippedHandEvent>(OnHandUnequipped);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<MoveToEntityComponent>();
        while (query.MoveNext(out var uid, out var component))
        {
            if (!component.Enabled)
                continue;

            MoveInRange(uid, component, Transform(uid).Coordinates, true);
        }
    }

    private void OnAfterInteract(EntityUid uid, MoveToEntityComponent component, ref AfterInteractEvent args)
    {
        if (!args.CanReach || args.Handled)
            return;

        MoveInRange(uid, component, args.ClickLocation);
        args.Handled = true;
    }

    private void OnInit(EntityUid uid, MoveToEntityComponent component, ref ComponentInit args)
    {
        if (component.EnableWhenHeld)
            return;

        component.LastPos = _transform.GetMapCoordinates(uid);
        Dirty(uid, component);
    }

    private void OnEquipped(EntityUid uid, MoveToEntityComponent component, ref GotEquippedEvent args)
    {
        ToggleEnabled(uid, component, true);
    }

    private void OnUnequipped(EntityUid uid, MoveToEntityComponent component, ref GotUnequippedEvent args)
    {
        ToggleEnabled(uid, component, false);
    }

    private void OnHandEquipped(EntityUid uid, MoveToEntityComponent component, ref GotEquippedHandEvent args)
    {
        ToggleEnabled(uid, component, true);
    }

    private void OnHandUnequipped(EntityUid uid, MoveToEntityComponent component, ref GotUnequippedHandEvent args)
    {
        ToggleEnabled(uid, component, false);
    }

    private void ToggleEnabled(EntityUid uid, MoveToEntityComponent component, bool enabled)
    {
        if (!component.EnableWhenHeld)
            return;

        component.Enabled = enabled;

        if (enabled)
            component.LastPos = _transform.GetMapCoordinates(Transform(uid).ParentUid);

        Dirty(uid, component);
    }

    private void MoveInRange(EntityUid uid, MoveToEntityComponent component, EntityCoordinates coords, bool checkAngle = false)
    {
        var posUid = uid;

        if (component.EnableWhenHeld)
            posUid = Transform(uid).ParentUid;

        var angle = _transform.GetWorldRotation(posUid);
        var uidPos = _transform.GetMapCoordinates(posUid);

        component.LastPos = uidPos;

        Dirty(uid, component);

        var entities = _entityLookup.GetEntitiesInRange(coords, component.Radius, LookupFlags.Dynamic | LookupFlags.Sundries);

        if (entities.Count <= 0)
            return;

        foreach (var ent in entities)
        {
            if (_whitelist.IsWhitelistFailOrNull(component.Whitelist, ent))
                continue;

            if (ent == uid || ent == posUid)
                continue;

            if (!TryComp<PhysicsComponent>(ent, out var physComp))
                continue;

            var entPos = _transform.GetMapCoordinates(ent);

            if (entPos.MapId != uidPos.MapId)
                continue;

            var dir = Vector2.Subtract(entPos.Position, uidPos.Position);

            if (checkAngle && Angle.ShortestDistance(dir.ToAngle(), angle) < Angle.FromDegrees(component.Tolerance))
            {
                continue;
            }

            if (dir.Length() != component.Offset)
            {
                _physics.SetLinearVelocity(ent, Vector2.Zero);

                var newPos = Vector2.Add(uidPos.Position, dir.Normalized() * component.Offset);
                var impulse = Vector2.Subtract(newPos, entPos.Position);

                var distance = Math.Abs(Vector2.Distance(uidPos.Position, component.LastPos.Position));
                var mass = physComp.Mass;

                // dont want to decress the magnitude so cap
                if (distance <= 1)
                    distance = 1;

                if (mass <= 1)
                    mass = 1;

                impulse *= distance;
                impulse *= mass;

                if (!checkAngle)
                {
                    impulse *= component.Scale;
                }
                else if(dir.Length() < component.Offset)
                {
                    // panic
                    // move out the fucking way
                    newPos = angle.ToWorldVec().Normalized() * component.Offset;
                    newPos += uidPos.Position;
                    var newCoords = new MapCoordinates(newPos, uidPos.MapId);
                    _transform.SetMapCoordinates(ent, newCoords);

                    var radius = component.Radius / 2;

                    if (radius <= 1)
                        radius = 1;
                }

                _physics.ApplyLinearImpulse(ent, impulse);
            }
        }
    }
}
