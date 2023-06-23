using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using COLID.AppDataService.Common.DataModel;

namespace COLID.AppDataService.Repositories.Configuration
{
    public class ApplicationVersionConfiguration : IEntityTypeConfiguration<ApplicationVersion>
    {
        public void Configure(EntityTypeBuilder<ApplicationVersion> builder)
        {
            builder.HasKey(t => new { t.Application, t.ReleaseDate });
        }
    }
}
