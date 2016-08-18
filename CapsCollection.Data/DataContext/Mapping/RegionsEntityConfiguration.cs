using System.Data.Entity.ModelConfiguration;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext.Mapping
{
    class RegionsEntityConfiguration : EntityTypeConfiguration<Geography_Region>
    {
        public RegionsEntityConfiguration()
        {
            this.HasKey(x => x.RegionID);

            this.Property(c => c.CountryID).IsRequired();
            this.Property(c => c.EnglishRegionName).HasMaxLength(100).IsRequired();
            this.Property(c => c.NationalRegionName).HasMaxLength(100).IsRequired();

            this.ToTable("Geography_Region", "geography");
        }
    }
}
