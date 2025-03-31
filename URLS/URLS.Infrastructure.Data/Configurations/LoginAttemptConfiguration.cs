using Extensions.DeviceDetector.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Extensions;

namespace URLS.Infrastructure.Data.Configurations;

public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
{
    public void Configure(EntityTypeBuilder<LoginAttempt> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.Login);
        builder.Property(x => x.Client).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<ClientInfo>());
    }
}
