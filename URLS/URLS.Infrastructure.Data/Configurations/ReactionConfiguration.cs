using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLS.Domain.Models;

namespace URLS.Infrastructure.Data.Configurations;

public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
{
    public void Configure(EntityTypeBuilder<Reaction> builder)
    {
        builder.HasOne(s => s.From).WithMany(s => s.Reactions)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
