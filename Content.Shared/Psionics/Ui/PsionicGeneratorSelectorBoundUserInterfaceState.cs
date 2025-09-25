using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared.Psionics.Ui;

[Serializable, NetSerializable]
public sealed partial class PsionicGeneratorSelectorBoundUserInterfaceState(ProtoId<PsionicDisiplinePrototype>? currentDisipline, HashSet<ProtoId<PsionicDisiplinePrototype>> disiplines) : BoundUserInterfaceState
{
    public readonly ProtoId<PsionicDisiplinePrototype>? CurrentDisipline = currentDisipline;
    public readonly HashSet<ProtoId<PsionicDisiplinePrototype>> Disiplines = disiplines;
}
