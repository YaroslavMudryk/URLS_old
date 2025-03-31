using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Extensions;

namespace URLS.Infrastructure.Data.Configurations;

public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
{
    public void Configure(EntityTypeBuilder<Quiz> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Config).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<QuizConfig>());

        builder.Property(s => s.Author).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<AuthorModel>());
    }
}
