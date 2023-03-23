namespace KfnApi.Abstractions;

public interface IStateful<TState> where TState : Enum
{
    TState State { get; set; }
}
