using KfnApi.Abstractions;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Services.Workflows;

public sealed class ApprovalFormWorkflow : IWorkflow<ApprovalFormState, ApprovalForm>
{
    private readonly Dictionary<ApprovalFormState, StateConfiguration<ApprovalFormTrigger, ApprovalFormState>> _machine = new();

    public ApprovalFormWorkflow()
    {
        ConfigureMachine();
    }

    public bool ApproveForm(ApprovalForm form, out ApprovalFormState? destination) => Fire(ApprovalFormTrigger.Approve, form, out destination);
    public bool DeclineForm(ApprovalForm form, out ApprovalFormState? destination) => Fire(ApprovalFormTrigger.Decline, form, out destination);

    public List<ApprovalFormState> NextPermittedStates(ApprovalForm form)
    {
        return WorkflowExtensions.GetConfiguration(_machine, form, out var configuration)
            ? configuration!.Transitions.Values.ToList()
            : new List<ApprovalFormState>();
    }

    private bool Fire(ApprovalFormTrigger trigger, ApprovalForm form, out ApprovalFormState? destination)
    {
        destination = null;

        if(!WorkflowExtensions.GetConfiguration(_machine, form, out var configuration))
            return false;

        if(!WorkflowExtensions.GetDestination(configuration!, trigger, out var result))
            return false;

        destination = result;

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
