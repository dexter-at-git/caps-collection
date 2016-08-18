using System.Data.Entity.ModelConfiguration;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext.Mapping
{
    class BeerEntityConfiguration : EntityTypeConfiguration<Beer_Beer>
    {
        public BeerEntityConfiguration()
        {
            this.HasKey(x => x.BeerId);

            this.Property(c => c.CountryId).IsRequired();
            this.Property(c => c.BeerName).HasMaxLength(50).IsRequired();
            this.Property(c => c.BeerType).HasMaxLength(50).IsOptional();
            this.Property(c => c.BeerStyleID).IsRequired();
            this.Property(c => c.BeerPrice).IsOptional();
            this.Property(c => c.BeerYear).IsOptional();
            this.Property(c => c.DateAdded).IsRequired();
            this.Property(c => c.BeerSite).HasMaxLength(100).IsOptional();
            this.Property(c => c.BeerComment).HasMaxLength(100).IsOptional();
            this.Property(c => c.BreweryId).IsRequired();
            this.Property(c => c.CapTypeID).IsRequired();

            this.ToTable("Beer_Beer", "capscollection");
        }
    }
}
