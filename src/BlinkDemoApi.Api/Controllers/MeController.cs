using System.Security.Claims;
using BlinkDemoApi.Application.Common;
using BlinkDemoApi.Application.Me.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlinkDemoApi.Api.Controllers;

/// <summary>
/// Endpoints related to the currently logged-in user.
/// </summary>
[Route("api/me")]
[Authorize]
public sealed class MeController : BaseApiController
{
    private readonly MeService _me;

    public MeController(MeService me)
    {
        _me = me;
    }

    [HttpGet("bootstrap")]
    public async Task<IActionResult> Bootstrap(CancellationToken ct)
    {
        // UserId from JWT (Guid/UUID).
        var sub = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var userId))
            return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        var data = await _me.GetBootstrapAsync(userId, ct);
        if (data is null)
            return Fail("User not found.", StatusCodes.Status404NotFound, ErrorCodes.NotFound);

        return OkResponse(data, "Bootstrap loaded.");
    }
}