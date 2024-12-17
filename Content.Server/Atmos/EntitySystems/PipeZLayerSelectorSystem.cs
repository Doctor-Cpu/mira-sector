using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Content.Shared.Atmos.Components;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Interaction;

namespace Content.Server.Atmos.EntitySystems;

public sealed partial class PipeZLayerSelectorSystem : SharedPipeZLayerSelectorSystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PipeZLayerSelectorComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<PipeZLayerSelectorComponent, ActivateInWorldEvent>(OnUseInHand);
    }

    private void OnInit(EntityUid uid, PipeZLayerSelectorComponent component, ComponentInit args)
    {
        UpdateZLayer(uid, component);
    }

    private void OnUseInHand(EntityUid uid, PipeZLayerSelectorComponent component, ActivateInWorldEvent args)
    {
        if (args.Handled)
            return;

        if (!args.Complex)
            return;

        args.Handled = UpdateZLayer(uid, component);
    }

    private bool UpdateZLayer(EntityUid uid, PipeZLayerSelectorComponent component)
    {
        if (!TryComp<NodeContainerComponent>(uid, out var nodeComp))
            return false;

        HashSet<int> zLayers = new();
        foreach (var (_, node) in nodeComp.Nodes)
        {
            if (node is not PipeNode pipe)
                continue;

            if (pipe.ZLayer == pipe.MaxZLayer)
            {
                pipe.ZLayer = pipe.MinZLayer;
            }
            else
            {
                pipe.ZLayer += 1;
            }

            zLayers.Add(pipe.ZLayer);
        }

        if (zLayers.Count <= 0)
            return false;

        foreach (var zLayer in zLayers)
        {
            if (component.ZLayer >= zLayer)
                continue;

            component.ZLayer = zLayer;
        }

        component.UiUpdate = true;
        Dirty(uid, component);
        return true;
    }
}
