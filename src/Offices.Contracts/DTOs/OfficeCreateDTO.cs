using Microsoft.AspNetCore.Http;
using Offices.Contracts.Enums;
using System.IO.Abstractions;

namespace Offices.Contracts.DTOs;

public record OfficeCreateDTO(string City, string Street, 
    string HouseNumber, string? OfficeNumber, string RegistryPhoneNumber, IFormFile? OfficePhoto, Status IsActive);
