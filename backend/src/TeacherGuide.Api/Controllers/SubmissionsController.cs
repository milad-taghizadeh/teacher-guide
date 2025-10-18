using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherGuide.Application.Interfaces;
using TeacherGuide.Application.Models.Submissions;
using TeacherGuide.Domain.Enums;

namespace TeacherGuide.Api.Controllers;

[ApiController]
[Route("api/submissions")]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionsController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<SubmissionResponse>> CreateSubmission([FromBody] SubmissionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _submissionService.CreateSubmissionAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SubmissionSummary>>> GetSubmissions([FromQuery] string? status, [FromQuery] string? search, [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken cancellationToken)
    {
        SubmissionStatus? parsedStatus = null;
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<SubmissionStatus>(status, true, out var value))
        {
            parsedStatus = value;
        }
        var filter = new SubmissionFilter(parsedStatus, search, from, to);
        var items = await _submissionService.GetSubmissionsAsync(filter, cancellationToken);
        return Ok(items);
    }

    [Authorize]
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute] Guid id, [FromBody] SubmissionStatusUpdate request, CancellationToken cancellationToken)
    {
        try
        {
            await _submissionService.UpdateStatusAsync(id, request, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
