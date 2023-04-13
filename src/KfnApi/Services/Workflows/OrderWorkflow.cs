using KfnApi.Abstractions;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Services.Workflows;

public sealed class OrderWorkflow : IWorkflow<OrderState, Order>
{
    private readonly Dictionary<OrderState, StateConfiguration<OrderTrigger, OrderState>> _machine = new();

    public OrderWorkflow()
    {
        ConfigureMachine();
    }

    public bool FailOrder(Order order, out OrderState? destination) => Fire(OrderTrigger.Fail, order, out destination);
    public bool CancelOrder(Order order, out OrderState? destination) => Fire(OrderTrigger.Cancel, order, out destination);
    public bool ExpireOrder(Order order, out OrderState? destination) => Fire(OrderTrigger.Expire, order, out destination);
    public bool ApproveOrder(Order order, out OrderState? destination) => Fire(OrderTrigger.Approve, order, out destination);
    public bool DeclineOrder(Order order, out OrderState? destination) => Fire(OrderTrigger.Decline, order, out destination);
    public bool ConcludeOrder(Order order, out OrderState? destination) =>  Fire(OrderTrigger.Conclude, order, out destination);
    public bool TerminateOrder(Order order, out OrderState? destination) => Fire(OrderTrigger.Terminate, order, out destination);

    public List<OrderState> NextPermittedStates(Order order)
    {
        return WorkflowExtensions.GetConfiguration(_machine, order, out var configuration)
            ? configuration!.Transitions.Values.ToList()
            : new List<OrderState>();
    }

    private bool Fire(OrderTrigger trigger, Order order, out OrderState? destination)
    {
        destination = null;

        if(!WorkflowExtensions.GetConfiguration(_machine, order, out var configuration))
            return false;

        if(!WorkflowExtensions.GetDestination(configuration!, trigger, out var result))
            return false;

        destination = result;

        return true;
    }

    private void ConfigureMachine()
    {
        _machine.Add(OrderState.Pending, new ()
        {
            Transitions = new()
            {
                { OrderTrigger.Approve, OrderState.Approved },
                { OrderTrigger.Decline, OrderState.Declined },
                { OrderTrigger.Cancel, OrderState.Cancelled }
            }
        });

        _machine.Add(OrderState.Approved, new()
        {
            Transitions = new()
            {
                { OrderTrigger.Cancel, OrderState.Cancelled },
                { OrderTrigger.Terminate, OrderState.Terminated },
                { OrderTrigger.Conclude, OrderState.Concluded },
                { OrderTrigger.Fail, OrderState.Failed },
                { OrderTrigger.Expire, OrderState.Indeterminate }
            }
        });
    }
}
