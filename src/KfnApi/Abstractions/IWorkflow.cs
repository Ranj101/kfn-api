namespace KfnApi.Abstractions;

public interface IWorkflow<TState, TValue> where TState : Enum where TValue : IStateful<TState>
{
    List<TState> NextPermittedStates(TValue value);
}
