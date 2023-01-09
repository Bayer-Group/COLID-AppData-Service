using COLID.AppDataService.Common.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace COLID.AppDataService.Repositories.Configuration
{
    public class FavoritesListConfiguration : IEntityTypeConfiguration<FavoritesList>
    {
        public void Configure(EntityTypeBuilder<FavoritesList> builder)
        {
            builder
                .HasIndex(fl => fl.Id)
                .IsUnique();

            builder
                .HasMany(fle => fle.FavoritesListEntries)
                .WithOne(fl => fl.FavoritesLists)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
