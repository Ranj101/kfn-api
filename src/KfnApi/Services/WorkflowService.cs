using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers.Authorization;
using KfnApi.Helpers.Database;
using KfnApi.Models.Common;
using KfnApi.Models.Entities;
using KfnApi.Models.Enums.Workflows;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using ZiggyCreatures.Caching.Fusion;

namespace KfnApi.Services;

public class WorkflowService : IWorkflowService
{
    private readonly IFusionCache _cache;
    private readonly IAuthContext _authContext;
    private readonly IUserService _userService;
    private readonly WorkflowContext _workflowContext;
    private readonly DatabaseContext _databaseContext;
    private readonly IProducerService _producerService;

    public WorkflowService(WorkflowContext workflowContext, DatabaseContext databaseContext, IAuthContext authContext,
        IFusionCache cache, IUserService userService, IProducerService producerService)
    {
        _cache = cache;
        _authContext = authContext;
        _userService = userService;
        _workflowContext = workflowContext;
        _databaseContext = databaseContext;
        _producerService = producerService;
    }

    public async Task<Result<User>> UpdateUserStateAsync(Guid id, UpdateUserStateRequest request)
    {
        var user = await _databaseContext.Users
            .Include(u => u.Producer)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return Result<User>.NotFoundResult();

        if (request.Trigger == UserTrigger.Deactivate)
            return await DeactivateUserAsync(user);

        return await ReactivateUserAsync(user);
    }

    public async Task<Result<Producer>> UpdateProducerStateAsync(Guid id, UpdateProducerStateRequest request)
    {
        var producer = await _databaseContext.Producers.FirstOrDefaultAsync(p => p.Id == id);

        if (producer is null)
            return Result<Producer>.NotFoundResult();

        if (request.Trigger == ProducerTrigger.Deactivate)
            return await DeactivateProducer(producer);

        return await ReactivateProducer(producer);
    }

    public async Task<Result<ApprovalForm>> UpdateFormStateAsync(Guid id, UpdateFormStateRequest request)
    {
        var form = await _databaseContext.ApprovalForms
            .Include(f => f.Uploads)
            .Include(f => f.User)
            .FirstOrDefaultAsync(f => f.Id == id);

        if(form is null)
            return Result<ApprovalForm>.NotFoundResult();

        if (request.Trigger == ApprovalFormTrigger.Decline)
            return await DeclineForm(form);

        return await ApproveForm(form);
    }

    private async Task<Result<User>> DeactivateUserAsync(User user)
    {
        if (!_workflowContext.UserWorkflow.DeactivateUser(user, out var destination))
            return Result<User>.StateErrorResult();

        var transaction = await _databaseContext.Database.BeginTransactionAsync();

        try
        {
            user.State = destination!.Value;
            user.UpdatedBy = _authContext.GetUserId();

            await _databaseContext.SaveChangesAsync();

            if (user.Producer is not null && user.Producer.State == ProducerState.Active)
            {
                var result = await DeactivateProducer(user.Producer, transaction);

                if (!result.IsSuccess())
                {
                    await transaction.RollbackAsync();
                    return Result<User>.ErrorResult(result.Error!);
                }

                await RemoveCacheAsync(user);
                return Result<User>.SuccessResult(user, StatusCodes.Status200OK);
            }

            await transaction.CommitAsync();
            await RemoveCacheAsync(user);
            return Result<User>.SuccessResult(user, StatusCodes.Status200OK);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<Result<User>> ReactivateUserAsync(User user)
    {
        if (!_workflowContext.UserWorkflow.ReactivateUser(user, out var destination))
            return Result<User>.StateErrorResult();

        user.State = destination!.Value;
        user.UpdatedBy = _authContext.GetUserId();

        await _databaseContext.SaveChangesAsync();

        return Result<User>.SuccessResult(user, StatusCodes.Status200OK);
    }

    private async Task<Result<Producer>> DeactivateProducer(Producer producer, IDbContextTransaction? transaction = null)
    {
        if (!_workflowContext.ProducerWorkflow.DeactivateProducer(producer, out var destination))
            return Result<Producer>.StateErrorResult();

        transaction ??= await _databaseContext.Database.BeginTransactionAsync();

        try
        {
            producer.State = destination!.Value;
            producer.UpdatedBy = _authContext.GetUserId();

            await _databaseContext.SaveChangesAsync();

            var roleUpdate = await _userService.UpdateUserRoleAsync(producer.UserId, Roles.Producer, remove:true, allowInactiveUser:true);

            if (!roleUpdate.IsSuccess())
            {
                await transaction.RollbackAsync();
                return Result<Producer>.ErrorResult(roleUpdate.Error!);
            }

            await transaction.CommitAsync();
            return Result<Producer>.SuccessResult(producer, StatusCodes.Status200OK);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<Result<Producer>> ReactivateProducer(Producer producer)
    {
        if (!_workflowContext.ProducerWorkflow.ReactivateProducer(producer, out var destination))
            return Result<Producer>.StateErrorResult();

        var transaction = await _databaseContext.Database.BeginTransactionAsync();

        try
        {
            producer.State = destination!.Value;
            producer.UpdatedBy = _authContext.GetUserId();

            await _databaseContext.SaveChangesAsync();

            var roleUpdate = await _userService.UpdateUserRoleAsync(producer.UserId, Roles.Producer);

            if (!roleUpdate.IsSuccess())
            {
                await transaction.RollbackAsync();
                return Result<Producer>.ErrorResult(roleUpdate.Error!);
            }

            await transaction.CommitAsync();
            return Result<Producer>.SuccessResult(producer, StatusCodes.Status200OK);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<Result<ApprovalForm>> ApproveForm(ApprovalForm form)
    {
        if (!_workflowContext.ApprovalFormWorkflow.ApproveForm(form, out var destination))
            return Result<ApprovalForm>.StateErrorResult();

        var transaction = await _databaseContext.Database.BeginTransactionAsync();

        try
        {
            form.State = destination!.Value;
            form.UpdatedBy = _authContext.GetUserId();

            await _databaseContext.SaveChangesAsync();

            var roleUpdate = await _userService.UpdateUserRoleAsync(form.UserId, Roles.Producer);

            if (!roleUpdate.IsSuccess())
            {
                await transaction.RollbackAsync();
                return Result<ApprovalForm>.ErrorResult(roleUpdate.Error!);
            }

            await _producerService.CreateProducerAsync(form);

            await transaction.CommitAsync();
            return Result<ApprovalForm>.SuccessResult(form, StatusCodes.Status200OK);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<Result<ApprovalForm>> DeclineForm(ApprovalForm form)
    {
        if (!_workflowContext.ApprovalFormWorkflow.DeclineForm(form, out var destination))
            return Result<ApprovalForm>.StateErrorResult();

        form.State = destination!.Value;
        form.UpdatedBy = _authContext.GetUserId();

        await _databaseContext.SaveChangesAsync();

        return Result<ApprovalForm>.SuccessResult(form, StatusCodes.Status200OK);
    }

    private async Task RemoveCacheAsync(User user)
    {
        await _cache.RemoveAsync($"user:{user.IdentityId}");
    }
}
