using System.Collections.Frozen;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Shared.Psionics;

public sealed partial class PsionicsSystem : EntitySystem
{
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _ui = default!;

    private FrozenDictionary<ProtoId<PsionicDisiplinePrototype>, PsionicDisiplinePrototype> _disiplines = default!;

    public override void Initialize()
    {
        base.Initialize();

        UpdateCachedDisiplines();

        InitializeGenerator();
        InitializeTerminal();

        SubscribeLocalEvent<PrototypesReloadedEventArgs>(OnPrototypeReload);
    }

    private void OnPrototypeReload(PrototypesReloadedEventArgs args)
    {
        if (args.WasModified<PsionicDisiplinePrototype>())
            UpdateCachedDisiplines();
    }

    private void UpdateCachedDisiplines()
    {
        var protos = _prototype.GetInstances<PsionicDisiplinePrototype>();
        Dictionary<ProtoId<PsionicDisiplinePrototype>, PsionicDisiplinePrototype> newDict = [];
        newDict.EnsureCapacity(protos.Count);
        foreach (var (id, proto) in protos)
            newDict[id] = proto;

        _disiplines = newDict.ToFrozenDictionary();
    }
}
