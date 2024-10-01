using Content.Shared.Access.Components;
using Content.Shared.PDA.Mesaging.Server;
using Robust.Shared.Containers;

namespace Content.Shared.PDA.Mesaging;

public abstract class PdaMessagingSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PdaMessagingComponent, EntInsertedIntoContainerMessage>(OnInsert);
    }

    private void OnInsert(EntityUid uid, PdaMessagingComponent component, ref EntInsertedIntoContainerMessage args)
    {
        if (!TryComp<IdCardComponent>(args.Entity, out var idComp) || idComp.FullName == null)
            return;

        var ev = new AttemptAddPdaReciever();
        ev.Uid = uid;
        ev.Name = idComp.FullName;

        RaiseLocalEvent(ev);
    }
}
