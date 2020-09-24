using COLID.AppDataService.Common.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace COLID.AppDataService.Repositories.Configuration
{
    public class WelcomeMessageConfiguration : IEntityTypeConfiguration<WelcomeMessage>
    {
        public void Configure(EntityTypeBuilder<WelcomeMessage> builder)
        {
            builder
                .HasIndex(wm => wm.Type)
                .IsUnique();
        }
    }
}
