using COLID.AppDataService.Common.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace COLID.AppDataService.Repositories.Configuration
{
    public class ConsumerGroupConfiguration : IEntityTypeConfiguration<ConsumerGroup>
    {
        public void Configure(EntityTypeBuilder<ConsumerGroup> builder)
        {
            builder
                .HasMany(u => u.Users)
                .WithOne(c => c.DefaultConsumerGroup)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
