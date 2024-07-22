using AutoMapper;
using Offices.Contracts.DTOs;
using Offices.Domain.Entities;
using Offices.Domain.Interfaces;
using Offices.Infrastructure.HttpClients;
using Offices.Services.Abstractions;
using OneOf;
using OneOf.Types;
using System.Net.Http;

namespace Offices.Services.Services;

public class OfficesService : IOfficesService
{
    private readonly IOfficesRepository _officesRepository;
    private readonly IMapper _mapper;

    public OfficesService(IOfficesRepository officesRepository, IMapper mapper)
    {
        _officesRepository = officesRepository;
        _mapper = mapper;
    }

    public async Task<OneOf<List<OfficeDetailsDTO>, NotFound>> GetAllOfficesAsync()
    {
        var offices = await _officesRepository.GetAllAsync();

        if (!offices.Any())
        {
            return new NotFound();
        }

        var mappedOfficesCollection = _mapper.Map<List<OfficeDetailsDTO>>(offices);

        return mappedOfficesCollection;
    }

    public async Task<OneOf<List<OfficeDetailsDTO>, NotFound>> GetOfficesByIdsAsync(IEnumerable<string> officesIds)
    {
        var offices = await _officesRepository.GetCollectionByIdsAsync(officesIds);

        if (!offices.Any())
        {
            return new NotFound();
        }

        var mappedOfficesCollection = _mapper.Map<List<OfficeDetailsDTO>>(offices);

        return mappedOfficesCollection;
    }

    public async Task<OneOf<OfficeDetailsDTO, NotFound>> GetOfficeByIdAsync(string officeId)
    {
        var office = await _officesRepository.GetByIdAsync(officeId);

        if (office is null)
        {
            return new NotFound();
        }

        var mappedOffice = _mapper.Map<OfficeDetailsDTO>(office);

        return mappedOffice;
    }

    public async Task<string> AddNewOfficeAsync(OfficeCreateDTO newOffice)
    {
        var mappedOffice = _mapper.Map<Office>(newOffice);

        await _officesRepository.AddNewAsync(mappedOffice);

        return mappedOffice.OfficeId;
    }

    public async Task<OneOf<Success, NotFound>> DeleteOfficeAsync(string officeId)
    {
        var office = await _officesRepository.GetByIdAsync(officeId);

        if (office is null)
        {
            return new NotFound();
        }

        await _officesRepository.DeleteAsync(officeId);

        return new Success();
    }

    public async Task<OneOf<Success, NotFound>> UpdateOfficeAsync(string officeId, OfficeUpdateDTO updatedOffice)
    {
        var office = await _officesRepository.GetByIdAsync(officeId);

        if (office is null)
        {
            return new NotFound();
        }

        _mapper.Map(updatedOffice,office);

        await _officesRepository.UpdateAsync(officeId, office);

        return new Success();
    }
}
