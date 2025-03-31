using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLS.Domain.Models;

namespace URLS.Infrastructure.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.User).WithMany(x => x.Comments).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Post).WithMany(x => x.Comments).OnDelete(DeleteBehavior.ClientSetNull);
    }
}
