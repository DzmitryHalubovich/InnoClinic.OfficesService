using Offices.Contracts.DTOs;
using OneOf;
using OneOf.Types;

namespace Offices.Services.Abstractions;

public interface IOfficesService
{
    public Task<OneOf<List<OfficeShortInfoDTO>, NotFound>> GetAllOfficesAsync();

    public Task<OneOf<List<OfficeDetailsDTO>, NotFound>> GetOfficesByIdsAsync(IEnumerable<string> officesIds);

    public Task<OneOf<OfficeDetailsDTO, NotFound>> GetOfficeByIdAsync(string officeId);

    public Task<string> AddNewOfficeAsync(OfficeCreateDTO newOffice);

    public Task<OneOf<Success, NotFound>> UpdateOfficeAsync(string officeId, OfficeUpdateDTO updatedOffice);

    public Task<OneOf<Success, NotFound>> DeleteOfficeAsync(string officeId);
}
