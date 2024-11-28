using Robust.Shared.GameStates;
using Robust.Shared.Map;

namespace Content.Shared.Silicons.StationAi;

[RegisterComponent, NetworkedComponent]
public sealed partial class StationAiMultiCamComponent : Component
{
    [DataField]
    public bool Enabled = false;

    [DataField]
    public uint MaxCameras = 6;

    [DataField]
    public string Parallax = "MultiCam";

    public Dictionary<uint, EntityUid> Cameras = new();

    public bool HasOverlay = false;
    public EntityCoordinates? EyeCoords;

    public EntityUid Map;
}
