using Content.Shared.Psionics.Ui;
using Robust.Client.UserInterface;

namespace Content.Client.Psionics.Ui;

public sealed partial class PsionicGeneratorSelectorBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private PsionicGeneratorSelectorWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<PsionicGeneratorSelectorWindow>();
        _window.Open();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is PsionicGeneratorSelectorBoundUserInterfaceState cast)
            _window?.UpdateState(cast);
    }
}
