using Content.Shared.Psionics.Ui;
using Robust.Client.UserInterface;

namespace Content.Client.Psionics.Ui;

public sealed partial class PsionicTerminalBoundUserInterface(EntityUid owner, Enum uiKey) : BoundUserInterface(owner, uiKey)
{
    private PsionicTerminalWindow? _window;

    protected override void Open()
    {
        base.Open();

        _window = this.CreateWindow<PsionicTerminalWindow>();
        _window.Open();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        if (state is PsionicTerminalBoundUserInterfaceState cast)
            _window?.UpdateState(cast);
    }
}
