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
            // This Converter will perform the conversion to and from Json to the desired
            builder.Property(f => f.QueryJson).HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<JObject>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            builder.Property(f => f.LastExecutionResult).HasConversion(
                v => JsonConvert.SerializeObject(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),
                v => JsonConvert.DeserializeObject<JObject>(v, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}
