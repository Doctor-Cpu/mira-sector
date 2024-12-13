using Content.Shared.Instruments;
using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.UserInterface;

namespace Content.Client.Instruments;

public sealed partial class RadioSystem : SharedRadioSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RadioHeadsetComponent, AfterActivatableUIOpenEvent>(OnUiOpen);

        SubscribeLocalEvent<RadioComponent, ItemToggledEvent>(OnRadioToggled);
        SubscribeLocalEvent<RadioComponent, RadioHostChangeEvent>(OnHostChange);
    }

    private void OnUiOpen(EntityUid uid, RadioHeadsetComponent component, AfterActivatableUIOpenEvent args)
    {
        if (!HasComp<InstrumentComponent>(uid))
            return;

        HeadsetToggle(uid);
    }

    private void OnRadioToggled(EntityUid uid, RadioComponent component, ref ItemToggledEvent args)
    {
        if (!TryComp<InstrumentComponent>(uid, out var instrument))
            return;

        RadioToggle(uid, component, instrument, ref args);
    }

    private void OnHostChange(EntityUid uid, RadioComponent component, RadioHostChangeEvent args)
    {
        if (!TryComp<InstrumentComponent>(uid, out var instrument))
            return;

        ChangePlayer(uid, GetEntity(args.Uid), component, instrument);
    }
}
