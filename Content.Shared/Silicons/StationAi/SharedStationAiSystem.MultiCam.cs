using Content.Shared.Parallax;

namespace Content.Shared.Silicons.StationAi;

public abstract partial class SharedStationAiSystem
{
    private void InitializeMulitCam()
    {
        SubscribeLocalEvent<StationAiMultiCamComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<StationAiMultiCamComponent, ComponentRemove>(OnRemove);
        SubscribeLocalEvent<StationAiMultiCamComponent, MultiCamEvent>(OnMultiCam);
    }

    private void OnInit(EntityUid uid, StationAiMultiCamComponent component, ComponentInit args)
    {
        component.HasOverlay = HasComp<StationAiOverlayComponent>(uid);

        var map = Maps.CreateMap(false);
        component.Map = map;

        //EnsureComp<ParallaxComponent>(map).Parallax = component.Parallax;
    }

    private void OnRemove(EntityUid uid, StationAiMultiCamComponent component, ComponentRemove args)
    {
        Del(component.Map);
    }

    private void OnMultiCam(EntityUid uid, StationAiMultiCamComponent component, ref MultiCamEvent args)
    {
        if (args.Handled)
            return;

        SetMultiCamMode(uid, !component.Enabled, component);
        args.Handled = true;
    }

    public void SetMultiCamMode(EntityUid uid, bool enabled, StationAiMultiCamComponent? multiCam = null)
    {
        if (!Resolve(uid, ref multiCam))
            return;

        if (!_containers.TryGetContainingContainer(uid, out var container) || !TryComp<StationAiCoreComponent>(container.Owner, out var coreComp))
            return;

        if (enabled)
            MultiCamEnable(uid, container.Owner, multiCam, coreComp);
        else
            MultiCamDisable(uid, container.Owner, multiCam, coreComp);

        multiCam.Enabled = enabled;
        Dirty(uid, multiCam);
    }

    internal void MultiCamEnable(EntityUid uid, EntityUid core, StationAiMultiCamComponent multiCam, StationAiCoreComponent coreComp)
    {
        multiCam.HasOverlay = HasComp<StationAiOverlayComponent>(uid);
        RemComp<StationAiOverlayComponent>(uid);

        multiCam.EyeCoords = Transform(core).Coordinates;
        ClearEye((core, coreComp));

        if (HasComp<EyeComponent>(uid))
            _eye.SetTarget(uid, multiCam.Map);
    }

    internal void MultiCamDisable(EntityUid uid, EntityUid core, StationAiMultiCamComponent multiCam, StationAiCoreComponent coreComp)
    {
        if (multiCam.HasOverlay)
            EnsureComp<StationAiOverlayComponent>(uid);

        if (multiCam.EyeCoords != null)
        {
            SetupEye((core, coreComp), multiCam.EyeCoords.Value);
            AttachEye((core, coreComp));
        }
    }
}
