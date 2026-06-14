using BlinkDemoApi.Application.Addresses.Dtos;
using BlinkDemoApi.Application.Addresses.Services;
using BlinkDemoApi.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlinkDemoApi.Api.Controllers;

[Route("api/addresses")]
[Authorize]
public sealed class AddressesController : BaseApiController
{
    private readonly AddressService _svc;

    public AddressesController(AddressService svc)
    {
        ArgumentNullException.ThrowIfNull(svc);
        _svc = svc;
    }

    [HttpGet]
    public async Task<IActionResult> GetMy(CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
            return InvalidToken();

        var data = await _svc.GetMyAddressesAsync(userId, ct);
        return OkResponse(data, "Addresses loaded.");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAddressRequest req, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
            return InvalidToken();

        var data = await _svc.CreateAsync(userId, req, ct);
        return OkResponse(data, "Address created.");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAddressRequest req, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
            return InvalidToken();

        var data = await _svc.UpdateAsync(userId, id, req, ct);
        if (data is null)
            return Fail("Address not found.", StatusCodes.Status404NotFound, ErrorCodes.NotFound);

        return OkResponse(data, "Address updated.");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
            return InvalidToken();

        var ok = await _svc.DeleteAsync(userId, id, ct);
        if (!ok)
            return Fail("Address not found.", StatusCodes.Status404NotFound, ErrorCodes.NotFound);

        return OkResponse(new { deleted = true }, "Address deleted.");
    }

    [HttpPost("{id:guid}/set-default")]
    public async Task<IActionResult> SetDefault(Guid id, CancellationToken ct)
    {
        if (!TryGetCurrentUserId(out var userId))
            return InvalidToken();

        var ok = await _svc.SetDefaultAsync(userId, id, ct);
        if (!ok)
            return Fail("Address not found.", StatusCodes.Status404NotFound, ErrorCodes.NotFound);

        return OkResponse(new { isDefaultSet = true }, "Default address set.");
    }
}