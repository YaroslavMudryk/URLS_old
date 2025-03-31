namespace URLS.Application.ViewModels.Lesson;

public class FullJournalViewModel
{
    public JournalStatic Statistics { get; set; }
    public List<StudentJournal> Students { get; set; }

    public static FullJournalViewModel GetBase()
    {
        return new FullJournalViewModel
        {
            Statistics = new JournalStatic
            {
                PercentOfAttendance = "0%"
            },
            Students = new List<StudentJournal>()
        };
    }
}
public class StudentJournal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double PercentOfAttendance { get; set; }
    public List<LessonMark> Lessons { get; set; }
}
public class LessonMark
{
    public DateTime Date { get; set; }
    public string Mark { get; set; }
}

public class JournalStatic
{
    public string PercentOfAttendance { get; set; }
}
