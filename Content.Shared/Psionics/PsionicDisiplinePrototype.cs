using Content.Shared.Psionics.Effects;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Shared.Psionics;

[Prototype]
public sealed partial class PsionicDisiplinePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; set; } = default!;

    [DataField(required: true)]
    public List<BasePsionicEffect> Effects = [];

    [DataField(required: true)]
    public LocId Name;

    [DataField(required: true)]
    public LocId Description;

    [DataField(required: true)]
    public SpriteSpecifier UnlockedIcon = default!;

    [DataField(required: true)]
    public SpriteSpecifier LockedIcon = default!;
}
