using AutoMapper;
using Offices.Contracts.DTOs;
using Offices.Domain.Entities;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<Office, OfficeDetailsDTO>();
        CreateMap<OfficeCreateDTO, Office>();
        CreateMap<OfficeUpdateDTO, Office>();

        CreateMap<Office, OfficeShortInfoDTO>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => 
            string.Join(", ", new[] 
            { 
                src.City, 
                src.Street, 
                src.HouseNumber, 
                src.OfficeNumber 
            }.Where(x => !string.IsNullOrEmpty(x)))));
    }
}
