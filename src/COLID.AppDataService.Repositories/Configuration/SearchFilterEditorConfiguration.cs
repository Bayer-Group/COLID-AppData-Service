using COLID.AppDataService.Common.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace COLID.AppDataService.Repositories.Configuration
{
    public class SearchFilterEditorConfiguration : IEntityTypeConfiguration<SearchFilterEditor>
    {
        public void Configure(EntityTypeBuilder<SearchFilterEditor> builder)
        {
            builder
                .HasMany(u => u.Users)
                .WithOne(c => c.SearchFilterEditor)
                .OnDelete(DeleteBehavior.SetNull);

            // This Converter will perform the conversion to and from Json to the desired type
            builder.Property(f => f.FilterJson).HasConversion(
                    v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                    v => JsonConvert.DeserializeObject<JObject>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}
