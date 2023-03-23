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

    public bool FailOrder(Order order) => Fire(OrderTrigger.Fail, order);
    public bool CancelOrder(Order order) => Fire(OrderTrigger.Cancel, order);
    public bool ExpireOrder(Order order) => Fire(OrderTrigger.Expire, order);
    public bool ApproveOrder(Order order) => Fire(OrderTrigger.Approve, order);
    public bool DeclineOrder(Order order) => Fire(OrderTrigger.Decline, order);
    public bool ConcludeOrder(Order order) =>  Fire(OrderTrigger.Conclude, order);
    public bool TerminateOrder(Order order) => Fire(OrderTrigger.Terminate, order);

    public List<OrderState> NextPermittedStates(Order order)
    {
        return WorkflowExtensions.GetConfiguration(_machine, order, out var configuration)
            ? configuration!.Transitions.Values.ToList()
            : new List<OrderState>();
    }

    private bool Fire(OrderTrigger trigger, Order order)
    {
        if(!WorkflowExtensions.GetConfiguration(_machine, order, out var configuration))
            return false;

        if(!WorkflowExtensions.GetDestination(configuration!, trigger, out var destination))
            return false;

        order.State = destination;

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
