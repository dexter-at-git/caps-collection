using System.Data.Entity.ModelConfiguration;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext.Mapping
{
    class BeerStyleEntityConfiguration : EntityTypeConfiguration<Beer_BeerStyle>
    {
        public BeerStyleEntityConfiguration()
        {
            this.HasKey(x => x.BeerStyleID);

            this.Property(x => x.BeerStyleName).HasMaxLength(50).IsRequired();

            this.ToTable("Beer_BeerStyle", "capscollection");
        }
    }
}
