using Extensions.DeviceDetector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Extensions;

namespace URLS.Infrastructure.Data.Configurations;

public class SessionConfiguration : IEntityTypeConfiguration<Session>
{
    public void Configure(EntityTypeBuilder<Session> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Client).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<ClientInfo>());

        builder.Property(x => x.Location).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<Location>());

        builder.Property(x => x.App).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<AppModel>());
    }
}
