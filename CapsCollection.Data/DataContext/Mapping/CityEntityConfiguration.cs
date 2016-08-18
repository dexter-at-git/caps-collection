using System.Data.Entity.ModelConfiguration;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext.Mapping
{
    class CityEntityConfiguration : EntityTypeConfiguration<Geography_City>
    {
        public CityEntityConfiguration()
        {
            this.HasKey(x => x.CityID);

            this.Property(c => c.RegionID).IsRequired();
            this.Property(c => c.EnglishCityName).HasMaxLength(100).IsRequired();
            this.Property(c => c.NationalCityName).HasMaxLength(100).IsRequired();

            this.ToTable("Geography_City", "geography");
        }
    }
}
