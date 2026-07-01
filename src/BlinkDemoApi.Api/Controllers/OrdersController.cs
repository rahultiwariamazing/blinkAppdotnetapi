using BlinkDemoApi.Application.Common;
using BlinkDemoApi.Application.Orders.Dtos;
using BlinkDemoApi.Application.Orders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlinkDemoApi.Api.Controllers;

[Route("api/orders")]
[Authorize]
public sealed class OrdersController : BaseApiController
{
    private readonly OrderService _svc;

    public OrdersController(OrderService svc)
    {
        _svc = svc;
    }

    [HttpGet]
    public async Task<IActionResult> GetMy(CancellationToken ct)
    {
        var uid = CurrentUserId;
        if (uid is null) return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        var data = await _svc.GetMyOrdersAsync(uid.Value, ct);
        return OkResponse(data, "Orders loaded.");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetDetail(Guid id, CancellationToken ct)
    {
        var uid = CurrentUserId;
        if (uid is null) return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        var data = await _svc.GetMyOrderDetailAsync(uid.Value, id, ct);
        if (data is null) return Fail("Order not found.", StatusCodes.Status404NotFound, ErrorCodes.NotFound);

        return OkResponse(data, "Order detail loaded.");
    }

    [HttpPost("place")]
    public async Task<IActionResult> Place([FromBody] PlaceOrderRequest req, CancellationToken ct)
    {
        var uid = CurrentUserId;
        if (uid is null) return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        var (ok, err, order) = await _svc.PlaceAsync(uid.Value, req, ct);
        if (!ok) return Fail(err ?? "Failed to place order.", StatusCodes.Status409Conflict, ErrorCodes.Conflict);

        return OkResponse(order!, "Order placed.");
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        var uid = CurrentUserId;
        if (uid is null) return Fail("Invalid token.", StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);

        var (ok, err) = await _svc.CancelAsync(uid.Value, id, ct);
        if (!ok) return Fail(err ?? "Failed to cancel order.", StatusCodes.Status409Conflict, ErrorCodes.Conflict);

        return OkResponse(new { cancelled = true }, "Order cancelled.");
    }
}