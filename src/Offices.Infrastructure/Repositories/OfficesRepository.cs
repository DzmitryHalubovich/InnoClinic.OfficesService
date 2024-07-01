using MongoDB.Driver;
using Offices.Domain.Entities;
using Offices.Domain.Interfaces;

namespace Offices.Infrastructure.Repositories;

public class OfficesRepository : IOfficesRepository
{
    private readonly IMongoCollection<Office> _officesCollection;

    public OfficesRepository(IMongoCollection<Office> officesCollection)
    {
        _officesCollection = officesCollection;
    }

    public async Task<List<Office>> GetAllAsync() => 
        await _officesCollection.Find(_ => true).ToListAsync();

    public async Task<List<Office>> GetCollectionByIdsAsync(IEnumerable<string> officesId) =>
        await _officesCollection.Find(x => officesId.Contains(x.OfficeId)).ToListAsync();

    public async Task<Office> GetByIdAsync(string officeId) => 
        await _officesCollection.Find(o => o.OfficeId.Equals(officeId)).FirstOrDefaultAsync();

    public async Task AddNewAsync(Office newOffice) =>
        await _officesCollection.InsertOneAsync(newOffice);

    public async Task DeleteAsync(string officeId) => 
        await _officesCollection.DeleteOneAsync(o => o.OfficeId.Equals(officeId));

    public async Task UpdateAsync(string officeId, Office updatedOffice) =>
        await _officesCollection.ReplaceOneAsync(o => o.OfficeId == officeId, updatedOffice);
}
