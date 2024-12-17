using Content.Client.Items;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;

namespace Content.Client.Atmos.EntitySystems;

public sealed partial class PipeZLayerSelectorSystem : SharedPipeZLayerSelectorSystem
{
    public override void Initialize()
    {
        base.Initialize();
        Subs.ItemStatus<PipeZLayerSelectorComponent>(ent => new PipeZLayerSelectorControl(ent));
    }
}
