using Content.Shared.Item;
using Content.Shared.Stainable;
using Robust.Client.GameObjects;

namespace Content.Client.Stainable;

public sealed partial class StainableSystem : SharedStainableSystem
{
    [Dependency] private readonly SharedItemSystem _item = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StainableComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<StainableComponent, AppearanceChangeEvent>(OnAppearanceChange);
    }

    private void OnMapInit(EntityUid uid, StainableComponent component, MapInitEvent args)
    {
        EnsureSpriteLayer(uid, component);
    }

    private void OnAppearanceChange(EntityUid uid, StainableComponent component, AppearanceChangeEvent args)
    {
        if (args.Sprite is not {} sprite)
            return;

        if (!sprite.LayerExists(StainLayer.Overlay, false))
            EnsureSpriteLayer(uid, component);

        if (_appearance.TryGetData<bool>(uid, StainVisuals.Enabled, out var enabled, args.Component))
        {
            sprite.LayerSetVisible(StainLayer.Overlay, enabled);

            // no point setting color
            if (!enabled)
            {
                _item.VisualsChanged(uid);
                return;
            }
        }

        if (_appearance.TryGetData<Color>(uid, StainVisuals.Color, out var color, args.Component))
        {
            sprite.LayerSetColor(StainLayer.Overlay, color);
        }

        _item.VisualsChanged(uid);
    }

    private void EnsureSpriteLayer(EntityUid uid, StainableComponent component)
    {
        if (component.Icon == null)
            return;

        if (!TryComp<SpriteComponent>(uid, out var sprite))
            return;

        var layer = sprite.LayerMapReserveBlank(StainLayer.Overlay);
        sprite.LayerSetSprite(layer, component.Icon);
    }
}
