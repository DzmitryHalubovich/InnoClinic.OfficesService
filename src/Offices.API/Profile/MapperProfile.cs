using AutoMapper;
using Offices.Contracts.DTOs;
using Offices.Domain.Entities;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Office, OfficeDetailsDTO>();
        CreateMap<OfficeCreateDTO, Office>();
        CreateMap<OfficeUpdateDTO, Office>()
            .ForMember(dest => dest.OfficeId, opt => opt.Ignore());
    }
}
