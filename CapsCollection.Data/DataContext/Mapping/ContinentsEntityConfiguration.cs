using System.Data.Entity.ModelConfiguration;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext.Mapping
{
    class ContinentsEntityConfiguration : EntityTypeConfiguration<Geography_Continent>
    {
        public ContinentsEntityConfiguration()
        {
            this.HasKey(x => x.ContinentID);

            this.Property(c => c.ContinentCode).HasMaxLength(2).IsRequired();
            this.Property(c => c.ContinentName).HasMaxLength(50).IsRequired();
            this.Property(c => c.EnglishContinentName).HasMaxLength(50).IsRequired();

            this.ToTable("Geography_Continent", "geography");
        }
    }
}
