namespace Content.Shared.PDA.Mesaging.Server;

public abstract class PdaMessagingServerSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PdaMessagingServerComponent, AttemptAddPdaReciever>(OnAddReciever);
    }

    private void OnAddReciever(EntityUid uid, PdaMessagingServerComponent component, ref AttemptAddPdaReciever args)
    {
        if (!TryComp<PdaMessagingComponent>(args.Uid, out var messagingComp))
            return;

        if (messagingComp.ConnectedServer != null && messagingComp.ConnectedServer != uid)
            return;

        bool hasUid = component.ConnectedPdas.ContainsKey(args.Uid);

        if (component.ConnectedPdas[args.Uid] == args.Name)
            return;

        if (hasUid)
        {
            component.ConnectedPdas[args.Uid] = args.Name;
        }
        else
        {
            // multiple people can have the same name so always add a new name to the database
            component.ConnectedPdas.Add(args.Uid, args.Name);

            messagingComp.ConnectedServer = uid;
            Dirty(args.Uid, messagingComp);
        }

        Dirty(uid, component);
    }
}
