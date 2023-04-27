using KfnApi.Abstractions;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;

namespace KfnApi.Services.Workflows;

public sealed class UserWorkflow : IWorkflow<UserState, User>
{
    private readonly Dictionary<UserState, StateConfiguration<UserTrigger, UserState>> _machine = new();

    public UserWorkflow()
    {
        ConfigureMachine();
    }

    public bool ReactivateUser(User user, out UserState? destination) => Fire(UserTrigger.Reactivate, user, out destination);
    public bool DeactivateUser(User user, out UserState? destination) => Fire(UserTrigger.Deactivate, user, out destination);

    public List<UserState> NextPermittedStates(User user)
    {
        return WorkflowExtensions.GetConfiguration(_machine, user, out var configuration)
            ? configuration!.Transitions.Values.ToList()
            : new List<UserState>();
    }

    private bool Fire(UserTrigger trigger, User user, out UserState? destination)
    {
        destination = null;

        if(!WorkflowExtensions.GetConfiguration(_machine, user, out var configuration))
            return false;

        if(!WorkflowExtensions.GetDestination(configuration!, trigger, out var result))
            return false;

        destination = result;

        return true;
    }

    private void ConfigureMachine()
    {
        _machine.Add(UserState.Active, new ()
        {
            Transitions = new()
            {
                { UserTrigger.Deactivate, UserState.Inactive }
            }
        });

        _machine.Add(UserState.Inactive, new()
        {
            Transitions = new()
            {
                { UserTrigger.Reactivate , UserState.Active }
            }
        });
    }
}
