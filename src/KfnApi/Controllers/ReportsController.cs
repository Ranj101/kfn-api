using KfnApi.Abstractions;
using KfnApi.DTOs.Requests;
using KfnApi.Helpers;
using KfnApi.Helpers.Extensions;
using KfnApi.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/report")]
public class ReportsController : KfnControllerBase
{
    private readonly IUserService _userService;
    private readonly IReportService _reportService;
    private readonly IProducerService _producerService;
    private readonly ICloudStorageService _cloudService;

    public ReportsController(IReportService reportService, IUserService userService, IProducerService producerService, ICloudStorageService cloudService)
    {
        _userService = userService;
        _reportService = reportService;
        _producerService = producerService;
        _cloudService = cloudService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReportsAsync([FromQuery] GetAllReportsRequest request)
    {
        if (request.AffiliatedEntityId is not null)
        {
            var paginatedUserRep = await _reportService.GetAllUserReportsByIdAsync(request);
            if (paginatedUserRep.Any())
            {
                var mapped = paginatedUserRep.Select(r => r.ToReportResponse()).ToList();
                return Ok(paginatedUserRep.ToPaginatedResponse(mapped));
            }

            var paginatedProducerRep = await _reportService.GetAllProducerReportsByIdAsync(request);
            if (paginatedProducerRep.Any())
            {
                var mapped = paginatedProducerRep.Select(p => p.ToReportResponse()).ToList();
                return Ok(paginatedUserRep.ToPaginatedResponse(mapped));
            }

            return Ok(paginatedProducerRep.ToPaginatedResponse(new List<object>()));
        }

        if (request.FilterByReportType == ReportType.UserReports)
        {
            var paginated = await _reportService.GetAllUserReportsAsync(request);
            var mapped = paginated.Select(r => r.ToReportResponse()).ToList();
            return Ok(paginated.ToPaginatedResponse(mapped));
        }

        if (request.FilterByReportType == ReportType.ProducerReports)
        {
            var paginated = await _reportService.GetAllProducerReportsAsync(request);
            var mapped = paginated.Select(r => r.ToReportResponse()).ToList();
            return Ok(paginated.ToPaginatedResponse(mapped));
        }

        var paginatedResponse = await _reportService.GetAllReportsAsync(request);

        return Ok(paginatedResponse.ToPaginatedResponse(paginatedResponse));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetReportAsync(Guid id)
    {
        var userReport = await _reportService.GetUserReportByIdAsync(id);

        if (userReport is not null)
            return Ok(userReport.ToUserReportResponse(_cloudService));

        var producerReport = await _reportService.GetProducerReportByIdAsync(id);

        return producerReport is null
            ? NotFoundResponse()
            : Ok(producerReport.ToProducerReportResponse());
    }

    [HttpPost("{id:guid}")]
    public async Task<IActionResult> SubmitReportAsync(Guid id, [FromBody] SubmitReportRequest request)
    {
        var user = await _userService.GetByIdAsync(id, activeOnly:true);

        if (user is not null)
        {
            var report = await _reportService.CreateUserReportAsync(user.Id, request);
            return Created("",report.ToUserReportResponse(_cloudService));
        }

        var producer = await _producerService.GetByIdAsync(id, activeOnly:true);

        if (producer is not null)
        {
            var report = await _reportService.CreateProducerReportAsync(producer.Id, request);
            return Created("", report.ToProducerReportResponse());
        }

        return NotFoundResponse();
    }
}
