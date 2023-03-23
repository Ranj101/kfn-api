using KfnApi.Abstractions;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Services.Workflows;

public sealed class ProducerFormWorkflow : IWorkflow<ProducerFormState, ProducerApprovalForm>
{
    private readonly Dictionary<ProducerFormState, StateConfiguration<ProducerFormTrigger, ProducerFormState>> _machine = new();

    public ProducerFormWorkflow()
    {
        ConfigureMachine();
    }

    public bool ApproveProducerForm(ProducerApprovalForm form) => Fire(ProducerFormTrigger.Approve, form);
    public bool DeclineProducerForm(ProducerApprovalForm form) => Fire(ProducerFormTrigger.Decline, form);

    public List<ProducerFormState> NextPermittedStates(ProducerApprovalForm form)
    {
        return WorkflowExtensions.GetConfiguration(_machine, form, out var configuration)
            ? configuration!.Transitions.Values.ToList()
            : new List<ProducerFormState>();
    }

    private bool Fire(ProducerFormTrigger trigger, ProducerApprovalForm form)
    {
        if(!WorkflowExtensions.GetConfiguration(_machine, form, out var configuration))
            return false;

        if(!WorkflowExtensions.GetDestination(configuration!, trigger, out var destination))
            return false;

        form.State = destination;

        return true;
    }

    private void ConfigureMachine()
    {
        _machine.Add(ProducerFormState.Pending, new ()
        {
            Transitions = new()
            {
                { ProducerFormTrigger.Approve, ProducerFormState.Approved },
                { ProducerFormTrigger.Decline, ProducerFormState.Declined }
            }
        });
    }
}
