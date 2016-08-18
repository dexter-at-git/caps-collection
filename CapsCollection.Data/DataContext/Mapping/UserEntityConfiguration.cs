using System.Data.Entity.ModelConfiguration;
using CapsCollection.Data.Models;

namespace CapsCollection.Data.DataContext.Mapping
{
    public class UserEntityConfiguration : EntityTypeConfiguration<User>
    {
        public UserEntityConfiguration()
        {
            this.HasKey(x => x.UserId);

            this.Property(c => c.UserName).HasMaxLength(50).IsRequired();
            this.Property(c => c.PasswordHash).HasMaxLength(128).IsRequired();
            this.Property(c => c.Salt).HasMaxLength(25).IsRequired();
            this.Property(c => c.UserType).IsRequired();
            this.Property(c => c.IsDisabled).IsRequired();

            this.ToTable("User", "user");
        }
    }
}