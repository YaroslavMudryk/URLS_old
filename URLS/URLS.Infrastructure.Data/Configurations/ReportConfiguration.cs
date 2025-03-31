using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Extensions;

namespace URLS.Infrastructure.Data.Configurations;

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CalculatedMarks).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<List<Student>>());

        builder.Property(s => s.Marks).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<List<Student>>());
    }
}
