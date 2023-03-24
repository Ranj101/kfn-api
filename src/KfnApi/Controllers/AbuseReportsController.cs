using KfnApi.Helpers;
using KfnApi.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KfnApi.Controllers;

[Authorize]
[ApiController]
[Route("v1/report")]
public class AbuseReportsController : KfnControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAbuseReportsAsync()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetAbuseReportAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    [HttpPost("user/{id:guid}")]
    public async Task<IActionResult> SubmitUserAbuseReportAsync(Guid id, SubmitAbuseReportRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpPost("producer/{id:guid}")]
    public async Task<IActionResult> SubmitProducerAbuseReportAsync(Guid id, SubmitAbuseReportRequest request)
    {
        throw new NotImplementedException();
    }
}
