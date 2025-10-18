using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeacherGuide.Application.Interfaces;
using TeacherGuide.Application.Models.Invites;

namespace TeacherGuide.Api.Controllers;

[ApiController]
[Route("api/invites")]
public class InvitesController : ControllerBase
{
    private readonly IInviteService _inviteService;
    private readonly IClock _clock;

    public InvitesController(IInviteService inviteService, IClock clock)
    {
        _inviteService = inviteService;
        _clock = clock;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<InviteResponse>> CreateInvite([FromBody] InviteRequest request, CancellationToken cancellationToken)
    {
        var response = await _inviteService.CreateInviteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetInvite), new { token = response.Token }, response);
    }

    [AllowAnonymous]
    [HttpGet("{token}")]
    public async Task<ActionResult<InviteDetails>> GetInvite([FromRoute] string token, CancellationToken cancellationToken)
    {
        var invite = await _inviteService.GetInviteAsync(token, cancellationToken);
        if (invite == null)
        {
            return NotFound();
        }
        if (invite.IsUsed || invite.ExpiresAt < _clock.UtcNow)
        {
            return StatusCode(StatusCodes.Status410Gone);
        }
        return Ok(invite);
    }
}
