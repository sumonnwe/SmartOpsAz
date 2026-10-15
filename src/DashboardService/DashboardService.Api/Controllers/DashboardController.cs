using DashboardService.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DashboardService.Api.Controllers;

/// <summary>
/// Dashboard API controller providing operational metrics and health summaries.
/// All endpoints require JWT authentication.
/// </summary>
[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
[Produces("application/json")]
public sealed class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(IMediator mediator, ILogger<DashboardController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get the dashboard summary with key operational metrics.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dashboard summary with aggregated metrics</returns>
    [HttpGet("summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DashboardSummaryResponse>> GetDashboardSummary(
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _mediator.Send(new GetDashboardSummaryQuery(), cancellationToken);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving dashboard summary");
            return StatusCode(500, new { error = "Internal server error", message = ex.Message });
        }
    }

    /// <summary>
    /// Get inventory dashboard data.
    /// NOT YET IMPLEMENTED - Placeholder for future inventory metrics.
    /// </summary>
    [HttpGet("inventory")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetInventoryDashboard()
    {
        return StatusCode(501, new 
        { 
            status = "not_implemented",
            message = "Inventory dashboard details not yet implemented",
            placeholder = true
        });
    }

    /// <summary>
    /// Get device health dashboard data.
    /// NOT YET IMPLEMENTED - Placeholder for future device health metrics.
    /// </summary>
    [HttpGet("device-health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetDeviceHealthDashboard()
    {
        return StatusCode(501, new 
        { 
            status = "not_implemented",
            message = "Device health dashboard details not yet implemented",
            placeholder = true
        });
    }

    /// <summary>
    /// Get temperature monitoring data.
    /// NOT YET IMPLEMENTED - Pending EnvironmentMonitoring service implementation.
    /// </summary>
    [HttpGet("temperature")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetTemperatureDashboard()
    {
        return Ok(new 
        { 
            status = "unavailable",
            message = "Temperature data not yet available",
            reason = "EnvironmentMonitoring service not yet implemented"
        });
    }

    /// <summary>
    /// Get door access events.
    /// NOT YET IMPLEMENTED - Pending AccessMonitoring service implementation.
    /// </summary>
    [HttpGet("door-events")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetDoorEvents()
    {
        return Ok(new 
        { 
            status = "unavailable",
            message = "Door events not yet available",
            reason = "AccessMonitoring service not yet implemented"
        });
    }

    /// <summary>
    /// Get active alerts.
    /// NOT YET IMPLEMENTED - Pending Alert service implementation.
    /// </summary>
    [HttpGet("alerts")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAlerts()
    {
        return Ok(new 
        { 
            status = "unavailable",
            message = "Alert data not yet available",
            reason = "Alert service not yet implemented"
        });
    }

    /// <summary>
    /// Get maintenance schedule and tickets.
    /// NOT YET IMPLEMENTED - Placeholder for future maintenance metrics.
    /// </summary>
    [HttpGet("maintenance")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public IActionResult GetMaintenance()
    {
        return StatusCode(501, new 
        { 
            status = "not_implemented",
            message = "Maintenance dashboard not yet implemented",
            placeholder = true
        });
    }

    /// <summary>
    /// Get AI-generated operational summary.
    /// Currently returns deterministic summary based on system health.
    /// Future: Integrate with Azure OpenAI for enhanced insights.
    /// </summary>
    [HttpGet("ai-summary")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAiSummary(CancellationToken cancellationToken)
    {
        try
        {
            var summary = await _mediator.Send(new GetDashboardSummaryQuery(), cancellationToken);
            return Ok(new 
            { 
                aiSummary = summary.OperationalSummary,
                generatedAtUtc = summary.GeneratedAtUtc,
                isAiGenerated = false,
                note = "Currently using deterministic summary. Future: Azure OpenAI integration."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving AI summary");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}