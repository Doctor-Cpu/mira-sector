using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Psionics.Ui;

[Serializable, NetSerializable]
public sealed partial class PsionicTerminalBoundUserInterfaceState(HashSet<ProtoId<PsionicDisiplinePrototype>> available, HashSet<ProtoId<PsionicDisiplinePrototype>> unlocked) : BoundUserInterfaceState
{
    public readonly HashSet<ProtoId<PsionicDisiplinePrototype>> Available = available;
    public readonly HashSet<ProtoId<PsionicDisiplinePrototype>> Unlocked = unlocked;
}
