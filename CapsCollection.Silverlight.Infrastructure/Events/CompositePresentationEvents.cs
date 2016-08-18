using CapsCollection.Silverlight.Infrastructure.Models;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Authentication;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Beer;
using CapsCollection.Silverlight.ServiceAgents.Proxies.Geography;
using Microsoft.Practices.Prism.Events;

namespace CapsCollection.Silverlight.Infrastructure.Events
{
    public class BeerCountrySelectedEvent : CompositePresentationEvent<CountryWithFlags> { }

    public class BeerAddedEvent : CompositePresentationEvent<BeerWithImages> { }

    public class CountryBeerAddedEvent : CompositePresentationEvent<int> { }
    public class CountryBeerDeletedEvent : CompositePresentationEvent<int> { }

    public class BeerCountriesReloadEvent : CompositePresentationEvent<bool> { }
    public class BeerDeletedEvent : CompositePresentationEvent<int> { }

    public class ShowBeerEditRegionEvent : CompositePresentationEvent<bool> { }
    public class ShowBeerListRegionEvent : CompositePresentationEvent<bool> { }

    public class IsAuthenticatedEvent : CompositePresentationEvent<AuthenticationData> { }

    public class BreweryFilterEvent : CompositePresentationEvent<BreweryFilter> { }
    public class BreweryAddedEvent : CompositePresentationEvent<BreweryDto> { }


    public class BreweriesReloadEvent : CompositePresentationEvent<bool> { }
    public class ShowBreweriesListRegionEvent : CompositePresentationEvent<bool> { }
    public class ShowBreweriesEditRegionEvent : CompositePresentationEvent<bool> { }
    

    public class ContinentSelectedEvent : CompositePresentationEvent<ContinentDto> { }
    public class CountrySelectedEvent : CompositePresentationEvent<CountryWithFlags> { }
    public class RegionSelectedEvent : CompositePresentationEvent<RegionDto> { }


    public class CountryEditingEvent : CompositePresentationEvent<CountryWithFlags> { }
    public class RegionEditingEvent : CompositePresentationEvent<CountryDto> { }
    public class CityEditingEvent : CompositePresentationEvent<CityDto> { }
    

    public class CountryDeletingEvent : CompositePresentationEvent<CountryWithFlags> { }
    public class RegionDeletingEvent : CompositePresentationEvent<RegionDto> { }
    public class CityDeletingEvent : CompositePresentationEvent<CityDto> { }


    public class CountryAddedEvent : CompositePresentationEvent<CountryWithFlags> { }
    public class RegionAddedEvent : CompositePresentationEvent<RegionDto> { }
    public class CityAddedEvent : CompositePresentationEvent<CityDto> { }
    public class BeerStyleAddedEvent : CompositePresentationEvent<BeerStyleDto> { }
}
