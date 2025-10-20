using MediatR;
using Microsoft.AspNetCore.Mvc;
using Report.Application.Features.Queries;

namespace ReportService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var result = await _sender.Send(new GetDashboardStatsQuery(from, to), ct);
        return Ok(result);
    }
}
