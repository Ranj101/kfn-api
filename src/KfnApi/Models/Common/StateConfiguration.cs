namespace KfnApi.Models.Common;

public sealed record StateConfiguration<TTrigger, TState> where TState : Enum where TTrigger : Enum
{
    public Dictionary<TTrigger, TState> Transitions { get; init; } = new();
}
