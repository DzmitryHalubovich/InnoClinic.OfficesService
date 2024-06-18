using Offices.Contracts.Enums;

namespace Offices.Contracts.DTOs;

public class OfficeShortInfoDTO()
{
    public string OfficeId { get; set; }

    public string? PhotoId { get; set; }

    public string Address { get; set; }

    public Status Status { get; set; }

    public string RegistryPhoneNumber { get; set; }
}
