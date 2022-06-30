using COLID.AppDataService.Common.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Repositories.Configuration
{
    public class StoredQueryConfiguration : IEntityTypeConfiguration<StoredQuery>
    {
        public void Configure(EntityTypeBuilder<StoredQuery> builder)
        {
            builder
               .HasOne(a => a.SearchFilterDataMarketplace)
               .WithOne(b => b.StoredQuery)
               .HasForeignKey<SearchFilterDataMarketplace>(b => b.StoredQueryId)
               .OnDelete(DeleteBehavior.SetNull);

        }
    }
}
