namespace BlinkDemoApi.Application.Addresses.Dtos;

public sealed record CreateAddressRequest(
    string Label,
    string AddressLine,
    string Pincode,
    string City,
    decimal? Lat,
    decimal? Lng,
    bool IsDefault
);

public sealed record UpdateAddressRequest(
    string Label,
    string AddressLine,
    string Pincode,
    string City,
    decimal? Lat,
    decimal? Lng,
    bool IsDefault
);

public sealed record AddressDto(
    Guid Id,
    string Label,
    string AddressLine,
    string Pincode,
    string City,
    decimal? Lat,
    decimal? Lng,
    bool IsDefault
);
