using KfnApi.Abstractions;
using KfnApi.Models.Common;

namespace KfnApi.Helpers.Extensions;

public static class WorkflowExtensions
{
    public static bool GetDestination<TTrigger, TState>(StateConfiguration<TTrigger, TState> configuration, TTrigger trigger,
        out TState? destination) where TState : Enum where TTrigger : Enum
        => configuration.Transitions.TryGetValue(trigger, out destination);
    
    public static bool GetConfiguration<TTrigger, TState, TValue>(Dictionary<TState, StateConfiguration<TTrigger, TState>> machine,
        TValue value, out StateConfiguration<TTrigger, TState>? configuration)
        where TState : Enum where TTrigger : Enum where TValue : IStateful<TState>
        => machine.TryGetValue(value.State, out configuration);
}
