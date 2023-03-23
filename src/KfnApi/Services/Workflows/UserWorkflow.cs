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

    public bool ReactivateUser(User user) => Fire(UserTrigger.Reactivate, user);
    public bool DeactivateUser(User user) => Fire(UserTrigger.Deactivate, user);

    public List<UserState> NextPermittedStates(User user)
    {
        return WorkflowExtensions.GetConfiguration(_machine, user, out var configuration)
            ? configuration!.Transitions.Values.ToList()
            : new List<UserState>();
    }

    private bool Fire(UserTrigger trigger, User user)
    {
        if(!WorkflowExtensions.GetConfiguration(_machine, user, out var configuration))
            return false;

        if(!WorkflowExtensions.GetDestination(configuration!, trigger, out var destination))
            return false;

        user.State = destination;

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
