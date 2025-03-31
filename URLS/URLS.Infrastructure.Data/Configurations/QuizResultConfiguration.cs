using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Extensions;

namespace URLS.Infrastructure.Data.Configurations;

public class QuizResultConfiguration : IEntityTypeConfiguration<QuizResult>
{
    public void Configure(EntityTypeBuilder<QuizResult> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Statistics).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<QuizResultStatistics>());

        builder.Property(s => s.Result).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<List<QuestionModel>>());
    }
}
