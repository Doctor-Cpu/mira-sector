using System.Diagnostics.CodeAnalysis;
using Content.Shared.Mind;
using Content.Shared.Psionics.Components;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;

namespace Content.Shared.Psionics;

public sealed partial class PsionicsSystem : EntitySystem
{
    [Dependency] private readonly SharedMindSystem _mind = default!;

    private void InitializeMind()
    {
    }

    [PublicAPI]
    public void MindLearnDisipline(EntityUid uid, ProtoId<PsionicDisiplinePrototype> displine)
    {
        if (!_mind.TryGetMind(uid, out var mindUid, out _))
            return;

        var mind = EnsureComp<PsionicMindComponent>(mindUid);
        if (!mind.LearnedDisiplines.Add(displine))
            return;

        Dirty(mindUid, mind);
    }

    [PublicAPI]
    public void MindUnlearnDisipline(EntityUid uid, ProtoId<PsionicDisiplinePrototype> displine)
    {
        if (!_mind.TryGetMind(uid, out var mindUid, out _))
            return;

        var mind = EnsureComp<PsionicMindComponent>(mindUid);
        if (!mind.LearnedDisiplines.Remove(displine))
            return;

        Dirty(mindUid, mind);
    }

    [PublicAPI]
    public bool TryGetUserLearnedDisiplines(EntityUid uid, [NotNullWhen(true)] out HashSet<ProtoId<PsionicDisiplinePrototype>>? disiplines)
    {
        if (_mind.TryGetMind(uid, out var mind, out _))
            return TryGetMindLearnedDisiplines(mind, out disiplines);

        disiplines = null;
        return false;
    }

    [PublicAPI]
    public bool TryGetMindLearnedDisiplines(Entity<PsionicMindComponent?> ent, [NotNullWhen(true)] out HashSet<ProtoId<PsionicDisiplinePrototype>>? disiplines)
    {
        if (!Resolve(ent.Owner, ref ent.Comp, false))
        {
            disiplines = null;
            return false;
        }

        disiplines = ent.Comp.LearnedDisiplines;
        return true;
    }
}
