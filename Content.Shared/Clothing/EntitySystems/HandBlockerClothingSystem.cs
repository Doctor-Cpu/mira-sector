using Content.Shared.Clothing.Components;
using Content.Shared.Hands;
using Content.Shared.Hands.Components;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Inventory.VirtualItem;
using Robust.Shared.Timing;

namespace Content.Shared.Clothing.EntitySystems;

public sealed partial class HandBlockerClothingSystem : EntitySystem
{
    [Dependency] private readonly SharedHandsSystem _hands = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedVirtualItemSystem _virtualItem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HandBlockerClothingComponent, BeingEquippedAttemptEvent>(OnEquipAttempt);
        SubscribeLocalEvent<HandBlockerClothingComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<HandBlockerClothingComponent, GotUnequippedEvent>(OnUnequipped);
        SubscribeLocalEvent<HandBlockerClothingComponent, VirtualItemDeletedEvent>(OnVirtualItemDeleted);
    }

    private void OnEquipAttempt(Entity<HandBlockerClothingComponent> ent, ref BeingEquippedAttemptEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<ClothingComponent>(ent.Owner, out var clothing) || (clothing.Slots & args.SlotFlags) == SlotFlags.NONE)
            return;

        var (emptyGenericHands, emptyHandsInLocations) = CountEmptyHands(ent, args.EquipTarget);

        if (IsHandsEmpty(ent, emptyGenericHands, emptyHandsInLocations))
            return;

        args.Reason = Loc.GetString("hand-blocker-clothing-blocked", ("item", ent));
        args.Cancel();
    }

    private void OnEquipped(Entity<HandBlockerClothingComponent> ent, ref GotEquippedEvent args)
    {
        for (var i = 0; i < ent.Comp.RequiredHands; i++)
            _virtualItem.TrySpawnVirtualItemInHand(ent.Owner, args.Equipee);

        Dictionary<HandLocation, int> locationalHandsCount = new(ent.Comp.RequiredHandsLocation.Count);
        foreach (var hand in _hands.EnumerateHands(args.Equipee))
        {
            if (!ent.Comp.RequiredHandsLocation.TryGetValue(hand.Location, out var requiredCount))
                continue;

            if (locationalHandsCount.TryGetValue(hand.Location, out var count))
            {
                count++;
                if (count >= requiredCount)
                    continue;
            }
            else
            {
                count = 1;
                locationalHandsCount[hand.Location] = count;
            }

            _virtualItem.TrySpawnVirtualItemInHand(ent.Owner, args.Equipee, out _, empty: hand);
        }

        ent.Comp.Wearer = args.Equipee;
        ent.Comp.Slot = args.Slot;
        Dirty(ent);
    }

    private void OnUnequipped(Entity<HandBlockerClothingComponent> ent, ref GotUnequippedEvent args)
    {
        _virtualItem.DeleteInHandsMatching(args.Equipee, ent.Owner);

        ent.Comp.Wearer = null;
        ent.Comp.Slot = null;
        Dirty(ent);
    }

    private void OnVirtualItemDeleted(Entity<HandBlockerClothingComponent> ent, ref VirtualItemDeletedEvent args)
    {
        if (args.BlockingEntity != ent.Owner || _timing.ApplyingState)
            return;

        if (ent.Comp.Wearer is not { } wearer || ent.Comp.Slot is not { } slot)
            return;

        if (_inventory.TryUnequip(wearer, slot, true, true, true))
            _hands.TryPickupAnyHand(wearer, ent.Owner);
    }

    private (int emptyGenericHands, Dictionary<HandLocation, int> emptyHandsInLocations) CountEmptyHands(Entity<HandBlockerClothingComponent> ent, EntityUid target)
    {
        Dictionary<HandLocation, int> emptyHandsInLocations = [];
        var emptyGenericHands = 0;

        foreach (var hand in _hands.EnumerateHands(target))
        {
            if (!hand.IsEmpty)
            {
                // wait thats us
                // dont count ourself
                if (hand.HeldEntity != ent.Owner)
                    continue;
            }

            if (ent.Comp.RequiredHandsLocation.ContainsKey(hand.Location))
                emptyHandsInLocations[hand.Location] = emptyHandsInLocations.GetValueOrDefault(hand.Location) + 1;
            else
                emptyGenericHands++;
        }

        return (emptyGenericHands, emptyHandsInLocations);
    }

    private static bool IsHandsEmpty(Entity<HandBlockerClothingComponent> ent, int emptyGenericHands, Dictionary<HandLocation, int> emptyHandsInLocations)
    {
        var requiredHands = ent.Comp.RequiredHands;

        foreach (var (location, requiredCount) in ent.Comp.RequiredHandsLocation)
        {
            if (emptyHandsInLocations.GetValueOrDefault(location) < requiredCount)
                return false;
        }

        return emptyGenericHands >= requiredHands;
    }
}
