using System.Data.Entity.ModelConfiguration;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext.Mapping
{
    class BreweryEntityConfiguration : EntityTypeConfiguration<Beer_Brewery>
    {
        public BreweryEntityConfiguration()
        {
            this.HasKey(x => x.BreweryId);

            this.Property(c => c.Brewery).HasMaxLength(50).IsRequired();
            this.Property(c => c.Site).HasMaxLength(50).IsOptional();
            this.Property(c => c.Comment).HasMaxLength(50).IsOptional();

            this.ToTable("Beer_Brewery", "capscollection");
        }
    }
}
