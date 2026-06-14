using BlinkDemoApi.Application.Addresses.Dtos;
using BlinkDemoApi.Application.Addresses.Services;
using BlinkDemoApi.Domain.Entities;
using BlinkDemoApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlinkDemoApi.Infrastructure.Repositories;

public sealed class AddressRepository : IAddressRepository
{
    private readonly AppDbContext _db;

    public AddressRepository(AppDbContext db)
    {
        ArgumentNullException.ThrowIfNull(db);
        _db = db;
    }

    public async Task<IReadOnlyList<AddressDto>> GetByUserAsync(Guid userId, CancellationToken ct)
    {
        return await _db.UserAddresses.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.Id)
            .Select(x => new AddressDto(x.Id, x.Label, x.AddressLine, x.Pincode, x.City, x.Lat, x.Lng, x.IsDefault))
            .ToListAsync(ct);
    }

    public async Task<AddressDto> CreateAsync(Guid userId, CreateAddressRequest req, CancellationToken ct)
    {
        if (req.IsDefault)
        {
            var oldDefaults = await _db.UserAddresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync(ct);

            foreach (var d in oldDefaults) d.IsDefault = false;
        }

        var row = new UserAddress
        {
            UserId = userId,
            Label = req.Label.Trim(),
            AddressLine = req.AddressLine.Trim(),
            Pincode = req.Pincode.Trim(),
            City = req.City.Trim(),
            Lat = req.Lat,
            Lng = req.Lng,
            IsDefault = req.IsDefault
        };

        _db.UserAddresses.Add(row);
        await _db.SaveChangesAsync(ct);

        return new AddressDto(row.Id, row.Label, row.AddressLine, row.Pincode, row.City, row.Lat, row.Lng, row.IsDefault);
    }

    public async Task<AddressDto?> UpdateAsync(Guid userId, Guid addressId, UpdateAddressRequest req, CancellationToken ct)
    {
        var row = await _db.UserAddresses
            .FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId, ct);

        if (row is null) return null;

        if (req.IsDefault)
        {
            var oldDefaults = await _db.UserAddresses
                .Where(a => a.UserId == userId && a.IsDefault && a.Id != addressId)
                .ToListAsync(ct);

            foreach (var d in oldDefaults) d.IsDefault = false;
        }

        row.Label = req.Label.Trim();
        row.AddressLine = req.AddressLine.Trim();
        row.Pincode = req.Pincode.Trim();
        row.City = req.City.Trim();
        row.Lat = req.Lat;
        row.Lng = req.Lng;
        row.IsDefault = req.IsDefault;

        await _db.SaveChangesAsync(ct);

        return new AddressDto(row.Id, row.Label, row.AddressLine, row.Pincode, row.City, row.Lat, row.Lng, row.IsDefault);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid addressId, CancellationToken ct)
    {
        var row = await _db.UserAddresses
            .FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId, ct);

        if (row is null) return false;

        _db.UserAddresses.Remove(row);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> SetDefaultAsync(Guid userId, Guid addressId, CancellationToken ct)
    {
        var row = await _db.UserAddresses
            .FirstOrDefaultAsync(x => x.Id == addressId && x.UserId == userId, ct);

        if (row is null) return false;

        var oldDefaults = await _db.UserAddresses
            .Where(a => a.UserId == userId && a.IsDefault && a.Id != addressId)
            .ToListAsync(ct);

        foreach (var d in oldDefaults) d.IsDefault = false;

        row.IsDefault = true;
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
