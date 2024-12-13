using Content.Shared.Item.ItemToggle.Components;

namespace Content.Shared.Instruments;

public abstract class SharedRadioSystem : EntitySystem
{
    private NetEntity? Host;

    public override void Initialize()
    {
        SubscribeLocalEvent<RadioHeadsetComponent, HeadphonesGetPlayerEvent>(OnGetPlayer);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (Host is not {} host)
            return;

        var ev = new RadioHostChangeEvent(host);

        var query = EntityQueryEnumerator<RadioComponent>();
        while (query.MoveNext(out var uid, out var radio))
        {
            RaiseLocalEvent(uid, ev);
        }

        // every radio is updated so discard
        Host = null;
    }

    private void OnGetPlayer(EntityUid uid, RadioHeadsetComponent component, HeadphonesGetPlayerEvent args)
    {
        args.Player = GetNetEntity(uid);
    }

    protected void HeadsetToggle(EntityUid uid)
    {
        Host = GetNetEntity(uid);
    }

    protected void RadioToggle(EntityUid uid, RadioComponent component, SharedInstrumentComponent instrument, ref ItemToggledEvent args)
    {
        if (args.Activated)
            instrument.Master = component.Host;
        else
            instrument.Master = null;

        Dirty(uid, instrument);
    }

    protected void ChangePlayer(EntityUid uid, EntityUid? player, RadioComponent component, SharedInstrumentComponent instrument)
    {
        instrument.Master = player;
        Dirty(uid, instrument);

        component.Host = player;
        Dirty(uid, component);
    }

}
