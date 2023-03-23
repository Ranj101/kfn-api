namespace KfnApi.Models.Enums.Workflow;

public enum OrderTrigger
{
    Fail,
    Cancel,
    Expire,
    Approve,
    Decline,
    Conclude,
    Terminate
}

public enum OrderState
{
    Failed,
    Pending,
    Approved,
    Declined,
    Cancelled,
    Concluded,
    Terminated,
    Indeterminate
}
