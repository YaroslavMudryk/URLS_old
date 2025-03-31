using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Extensions;

namespace URLS.Infrastructure.Data.Configurations;

public class TimetableConfiguration : IEntityTypeConfiguration<Timetable>
{
    public void Configure(EntityTypeBuilder<Timetable> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Holiday).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<Holiday>());

        builder.Property(x => x.Time).HasConversion(
            v => v.ToJson(),
            v => v.FromJson<LessonTime>());

        builder.HasOne(x => x.Teacher).WithMany(x => x.Timetables).OnDelete(DeleteBehavior.NoAction);
        builder.HasOne(x => x.Subject).WithMany(x => x.Timetables).OnDelete(DeleteBehavior.Cascade);
    }
}
