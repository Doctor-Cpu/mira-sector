namespace Content.Shared.Psionics.Events;

[ByRefEvent]
public record struct GetPsionicGeneratorUserEvent()
{
    public EntityUid? User;
}
