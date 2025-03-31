using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Extensions;

namespace URLS.Infrastructure.Data.Configurations;

public class UserGroupRoleConfiguration : IEntityTypeConfiguration<UserGroupRole>
{
    public void Configure(EntityTypeBuilder<UserGroupRole> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Permissions).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<UserGroupPermission>());
    }
}
