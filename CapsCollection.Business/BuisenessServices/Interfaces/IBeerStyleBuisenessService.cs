using System.Collections.Generic;
using CapsCollection.Business.DTOs;

namespace CapsCollection.Business.BuisenessServices.Interfaces
{
    public interface IBeerStyleBuisenessService 
    {
        List<BeerStyleDto> GetBeerStyles();
        BeerStyleDto GetBeerStyle(int beerStyleId);
        int SaveBeerStyle(BeerStyleDto beerStyle);
        void DeleteBeerStyle(BeerStyleDto beerStyle);
    }
}
