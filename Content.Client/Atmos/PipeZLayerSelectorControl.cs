using Content.Client.Message;
using Content.Client.Stylesheets;
using Content.Shared.Atmos.Components;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client.Atmos;

public sealed class PipeZLayerSelectorControl : Control
{
    private readonly PipeZLayerSelectorComponent _parent;
    private readonly RichTextLabel _label;

    public PipeZLayerSelectorControl(PipeZLayerSelectorComponent parent)
    {
        _parent = parent;
        _label = new RichTextLabel {StyleClasses = {StyleNano.StyleClassItemStatus}};
        _label.SetMarkup(Loc.GetString("pipe-zlayer-status", ("zlayer", _parent.ZLayer)));
        AddChild(_label);
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (!_parent.UiUpdate)
        {
            return;
        }

        _parent.UiUpdate = false;
        _label.SetMarkup(Loc.GetString("pipe-zlayer-status", ("zlayer", _parent.ZLayer)));
    }
}
