using Robust.Shared.GameStates;
namespace Content.Shared.Atmos.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class PipeZLayerSelectorComponent : Component
{
    public bool UiUpdate = false;
    public int ZLayer = 0;
}
