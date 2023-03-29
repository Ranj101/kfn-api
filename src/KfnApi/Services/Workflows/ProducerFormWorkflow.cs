using KfnApi.Abstractions;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Services.Workflows;

public sealed class ProducerFormWorkflow : IWorkflow<ApprovalFormState, ApprovalForm>
{
    private readonly Dictionary<ApprovalFormState, StateConfiguration<ApprovalFormTrigger, ApprovalFormState>> _machine = new();

    public ProducerFormWorkflow()
    {
        ConfigureMachine();
    }

    public bool ApproveForm(ApprovalForm form) => Fire(ApprovalFormTrigger.Approve, form);
    public bool DeclineForm(ApprovalForm form) => Fire(ApprovalFormTrigger.Decline, form);

    public List<ApprovalFormState> NextPermittedStates(ApprovalForm form)
    {
        return WorkflowExtensions.GetConfiguration(_machine, form, out var configuration)
            ? configuration!.Transitions.Values.ToList()
            : new List<ApprovalFormState>();
    }

    private bool Fire(ApprovalFormTrigger trigger, ApprovalForm form)
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
        _machine.Add(ApprovalFormState.Pending, new ()
        {
            Transitions = new()
            {
                { ApprovalFormTrigger.Approve, ApprovalFormState.Approved },
                { ApprovalFormTrigger.Decline, ApprovalFormState.Declined }
            }
        });
    }
}
