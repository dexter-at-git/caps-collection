using AutoMapper;
using CapsCollection.Data.Models;

namespace CapsCollection.Business.DTOs.MapperProfiles
{
    public class GeographyMapperProfile : Profile
    {
        protected override void Configure()
        {
#pragma warning disable 618
            Mapper.CreateMap<Geography_Country, CountryDto>();
            Mapper.CreateMap<CountryDto, Geography_Country>();

            Mapper.CreateMap<Geography_Continent, ContinentDto>();
            Mapper.CreateMap<ContinentDto, Geography_Continent>();

            Mapper.CreateMap<Geography_Region, RegionDto>();
            Mapper.CreateMap<RegionDto, Geography_Region>();

            Mapper.CreateMap<Geography_City, CityDto>();
            Mapper.CreateMap<CityDto, Geography_City>();
#pragma warning restore 618
        }
    }
}
