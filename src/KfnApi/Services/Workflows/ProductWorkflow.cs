using KfnApi.Abstractions;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Services.Workflows;

public sealed class ProductWorkflow : IWorkflow<ProductState, Product>
{
    private readonly Dictionary<ProductState, StateConfiguration<ProductTrigger, ProductState>> _machine = new();

    public ProductWorkflow()
    {
        ConfigureMachine();
    }

    public bool MakeAvailable(Product product, out ProductState? destination) => Fire(ProductTrigger.MakeAvailable, product, out destination);
    public bool MakeUnavailable(Product product, out ProductState? destination) => Fire(ProductTrigger.MakeUnavailable, product, out destination);
    public bool MarkAsModified(Product product, out ProductState? destination) => Fire(ProductTrigger.MarkAsModified, product, out destination);

    public List<ProductState> NextPermittedStates(Product product)
    {
        return WorkflowExtensions.GetConfiguration(_machine, product, out var configuration)
            ? configuration!.Transitions.Values.ToList()
            : new List<ProductState>();
    }

    private bool Fire(ProductTrigger trigger, Product product, out ProductState? destination)
    {
        destination = null;

        if(!WorkflowExtensions.GetConfiguration(_machine, product, out var configuration))
            return false;

        if(!WorkflowExtensions.GetDestination(configuration!, trigger, out var result))
            return false;

        destination = result;

        return true;
    }

    private void ConfigureMachine()
    {
        _machine.Add(ProductState.Available, new ()
        {
            Transitions = new()
            {
                { ProductTrigger.MakeUnavailable, ProductState.Unavailable },
                { ProductTrigger.MarkAsModified , ProductState.Modified }
            }
        });

        _machine.Add(ProductState.Unavailable, new ()
        {
            Transitions = new()
            {
                { ProductTrigger.MakeAvailable, ProductState.Available },
                { ProductTrigger.MarkAsModified , ProductState.Modified }
            }
        });
    }
}
