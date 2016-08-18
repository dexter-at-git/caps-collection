using AutoMapper;
using CapsCollection.Common.Extensions;
using CapsCollection.Data.Models;

namespace CapsCollection.Business.DTOs.MapperProfiles
{
    public class BeerMapperProfile : Profile
    {
        protected override void Configure()
        {
#pragma warning disable 618
            Mapper.CreateMap<Beer_Beer, BeerDto>()
                   .ForMember(dest => dest.ContinentId, opts => opts.MapFrom(src => src.Country.ContinentID))
                   .ForMember(dest => dest.BeerNameNoDiacritics, opts => opts.MapFrom(src => src.BeerName.RemoveDiacritics()));

            Mapper.CreateMap<BeerDto, Beer_Beer>();

            Mapper.CreateMap<Beer_BeerStyle, BeerStyleDto>();
            Mapper.CreateMap<BeerStyleDto, Beer_BeerStyle>();

            Mapper.CreateMap<Beer_CapType, CapTypeDto>();
            Mapper.CreateMap<CapTypeDto, Beer_CapType>();

            Mapper.CreateMap<Geography_Country, BeerCountryDto>();
            Mapper.CreateMap<BeerCountryDto, Geography_Country>();

            Mapper.CreateMap<Beer_Brewery, BreweryDto>()
                .ForMember(dest => dest.CityId, opts => opts.MapFrom(src => src.City.CityID))
                .ForMember(dest => dest.RegionId, opts => opts.MapFrom(src => src.City.Region.RegionID))
                .ForMember(dest => dest.CountryId, opts => opts.MapFrom(src => src.City.Region.Country.CountryID))
                .ForMember(dest => dest.ContinentId, opts => opts.MapFrom(src => src.City.Region.Country.ContinentID));

            Mapper.CreateMap<BreweryDto, Beer_Brewery>();
#pragma warning restore 618
        }
    }
}
