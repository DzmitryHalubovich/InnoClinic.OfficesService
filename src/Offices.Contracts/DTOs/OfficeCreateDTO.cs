using Offices.Contracts.Enums;

namespace Offices.Contracts.DTOs;

public record OfficeCreateDTO(string? OfficePhotoUrl, string City,
    string Street, string HouseNumber, string? OfficeNumber, string RegistryPhoneNumber, Status IsActive);
