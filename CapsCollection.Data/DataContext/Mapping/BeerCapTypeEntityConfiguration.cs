using System.Data.Entity.ModelConfiguration;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext.Mapping
{
    public class BeerCapTypeEntityConfiguration : EntityTypeConfiguration<Beer_CapType>
    {
        public BeerCapTypeEntityConfiguration()
        {
            this.HasKey(x => x.CapTypeId);

            this.Property(x => x.CapTypeName).HasMaxLength(50).IsRequired();

            this.ToTable("Beer_CapType", "capscollection");
        }
    }
}