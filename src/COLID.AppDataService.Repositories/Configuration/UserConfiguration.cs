using COLID.AppDataService.Common.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace COLID.AppDataService.Repositories.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder
                .HasIndex(u => u.Id)
                .IsUnique();

            builder
                .HasMany(u => u.SearchFiltersDataMarketplace)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.Messages)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.ColidEntrySubscriptions)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(u => u.FavoritesLists)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
