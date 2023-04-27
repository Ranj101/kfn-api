using KfnApi.Services.Workflows;

namespace KfnApi.Services;

public sealed class WorkflowContext
{
    public readonly UserWorkflow UserWorkflow;
    public readonly OrderWorkflow OrderWorkflow;
    public readonly ProductWorkflow ProductWorkflow;
    public readonly ProducerWorkflow ProducerWorkflow;
    public readonly ApprovalFormWorkflow ApprovalFormWorkflow;

    public WorkflowContext()
    {
        UserWorkflow = new UserWorkflow();
        OrderWorkflow = new OrderWorkflow();
        ProductWorkflow = new ProductWorkflow();
        ProducerWorkflow = new ProducerWorkflow();
        ApprovalFormWorkflow = new ApprovalFormWorkflow();
    }
}
