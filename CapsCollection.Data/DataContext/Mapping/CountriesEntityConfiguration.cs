using System.Data.Entity.ModelConfiguration;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext.Mapping
{
    class CountriesEntityConfiguration : EntityTypeConfiguration<Geography_Country>
    {
        public CountriesEntityConfiguration()
        {
            this.HasKey(x => x.CountryID);

            this.Property(c => c.ContinentID).IsRequired();
            this.Property(c => c.EnglishCountryName).HasMaxLength(100).IsRequired();
            this.Property(c => c.EnglishCountryFullName).HasMaxLength(100).IsOptional();
            this.Property(c => c.NationalCountryName).HasMaxLength(100).IsRequired();
            this.Property(c => c.NationalCountryFullName).HasMaxLength(100).IsOptional();
            this.Property(c => c.Alpha2).HasMaxLength(2).IsOptional();
            this.Property(c => c.Alpha3).HasMaxLength(3).IsOptional();
            this.Property(c => c.ISO).HasMaxLength(3).IsOptional();
            this.Property(c => c.PreciseLocation).HasMaxLength(100).IsOptional();

            this.ToTable("Geography_Country", "geography");
        }
    }
}
