using KfnApi.Abstractions;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Services.Workflows;

public sealed class ProducerWorkflow : IWorkflow<ProducerState, Producer>
{
    private readonly Dictionary<ProducerState, StateConfiguration<ProducerTrigger, ProducerState>> _machine = new();

    public ProducerWorkflow()
    {
        ConfigureMachine();
    }

    public bool ReactivateProducer(Producer producer, out ProducerState? destination) => Fire(ProducerTrigger.Reactivate, producer, out destination);
    public bool DeactivateProducer(Producer producer, out ProducerState? destination) => Fire(ProducerTrigger.Deactivate, producer, out destination);

    public List<ProducerState> NextPermittedStates(Producer producer)
    {
        return WorkflowExtensions.GetConfiguration(_machine, producer, out var configuration)
            ? configuration!.Transitions.Values.ToList()
            : new List<ProducerState>();
    }

    private bool Fire(ProducerTrigger trigger, Producer producer, out ProducerState? destination)
    {
        destination = null;

        if(!WorkflowExtensions.GetConfiguration(_machine, producer, out var configuration))
            return false;

        if(!WorkflowExtensions.GetDestination(configuration!, trigger, out var result))
            return false;

        destination = result;

        return true;
    }

    private void ConfigureMachine()
    {
        _machine.Add(ProducerState.Active, new ()
        {
            Transitions = new()
            {
                { ProducerTrigger.Deactivate, ProducerState.Inactive }
            }
        });

        _machine.Add(ProducerState.Inactive, new()
        {
            Transitions = new()
            {
                { ProducerTrigger.Reactivate , ProducerState.Active }
            }
        });
    }
}
