using System.Data.Entity;
using CapsCollection.Common.Settings;
using CapsCollection.Data.DataContext.Mapping;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext
{
    public class CapsCollectionContext : DbContext
    {
        public CapsCollectionContext() : base(CapsCollectionSettings.GetEntityConnectionString)
        {
            var ensureDLLIsCopied = System.Data.Entity.SqlServer.SqlProviderServices.Instance;
        }

        public IDbSet<Geography_Continent> Continents { get; set; }
        public IDbSet<Geography_Country> Countries { get; set; }
        public IDbSet<Geography_Region> Regions { get; set; }
        public IDbSet<Geography_City> Cities { get; set; }
        public IDbSet<Beer_Beer> Beers { get; set; }
        public IDbSet<Beer_Brewery> Breweries { get; set; }
        public IDbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserEntityConfiguration());
            modelBuilder.Configurations.Add(new ContinentsEntityConfiguration());
            modelBuilder.Configurations.Add(new CountriesEntityConfiguration());
            modelBuilder.Configurations.Add(new RegionsEntityConfiguration());
            modelBuilder.Configurations.Add(new CityEntityConfiguration());
            modelBuilder.Configurations.Add(new BreweryEntityConfiguration());
            modelBuilder.Configurations.Add(new BeerEntityConfiguration());
            modelBuilder.Configurations.Add(new BeerStyleEntityConfiguration());
            modelBuilder.Configurations.Add(new BeerCapTypeEntityConfiguration());
        }
    }
}
