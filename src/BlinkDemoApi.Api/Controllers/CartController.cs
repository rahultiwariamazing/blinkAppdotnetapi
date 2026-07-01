using BlinkDemoApi.Application.Cart.Dtos;
using BlinkDemoApi.Application.Cart.Services;
using BlinkDemoApi.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlinkDemoApi.Api.Controllers;

[Route("api/cart")]
[Authorize]
public sealed class CartController : BaseApiController
{
    private readonly CartService _svc;

    public CartController(CartService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<IActionResult> GetMy(CancellationToken ct)
    {
        var uid = CurrentUserId;
        if (uid is null) return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        var cart = await _svc.GetMyCartAsync(uid.Value, ct);
        return OkResponse(cart, "Cart loaded.");
    }

    [HttpPost("upsert")]
    public async Task<IActionResult> Upsert([FromBody] UpsertCartItemRequest req, CancellationToken ct)
    {
        var uid = CurrentUserId;
        if (uid is null) return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        var (ok, err, cart) = await _svc.UpsertAsync(uid.Value, req, ct);
        if (!ok) return Fail(err ?? "Failed.", StatusCodes.Status409Conflict, ErrorCodes.Conflict);

        return OkResponse(cart, "Cart updated.");
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Remove(Guid productId, CancellationToken ct)
    {
        var uid = CurrentUserId;
        if (uid is null) return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        var ok = await _svc.RemoveAsync(uid.Value, productId, ct);
        if (!ok) return Fail("Cart item not found.", StatusCodes.Status404NotFound, ErrorCodes.NotFound);

        var cart = await _svc.GetMyCartAsync(uid.Value, ct);
        return OkResponse(cart, "Cart item removed.");
    }

    [HttpPost("clear")]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        var uid = CurrentUserId;
        if (uid is null) return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        await _svc.ClearAsync(uid.Value, ct);
        return OkResponse(new { cleared = true }, "Cart cleared.");
    }
}