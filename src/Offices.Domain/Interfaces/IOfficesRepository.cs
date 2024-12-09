using Offices.Domain.Entities;

namespace Offices.Domain.Interfaces;

public interface IOfficesRepository
{
    public Task<List<Office>> GetAllAsync();
    public Task<List<Office>> GetCollectionByIdsAsync(IEnumerable<string> officesId);
    public Task<Office> GetByIdAsync(string officeId);
    public Task AddNewAsync(Office newOffice);
    public Task UpdateAsync(string officeId, Office updatedOffice);
    public Task DeleteAsync(string officeId);
}
