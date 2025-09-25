using Robust.Shared.Prototypes;

namespace Content.Shared.Psionics.Events;

[ByRefEvent]
public record struct GetPsionicGeneratorDisiplinesEvent()
{
    public readonly HashSet<ProtoId<PsionicDisiplinePrototype>> Disiplines = [];
    public ProtoId<PsionicDisiplinePrototype>? CurrentDisipline;
}
