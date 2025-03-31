namespace URLS.Domain.Models;

public class SubjectConfig
{
    public int RecommendedForCourse { get; set; }
    public double MaxMarkPerLesson { get; set; }
    public double MinMarkPerLesson { get; set;}
    public bool WithExam { get; set; }
    public double MaxMark { get; set; }
    public double MaxMarkUpToExam { get; set; }
    public double MaxMarkInExam { get; set; }
    public int CommonHours { get; set; }
    public int LectureHours { get; set; }
    public int PracticHours { get; set; }
    public int LabsHours { get; set; }
    public int IndividualHours { get; set; }
    public int ExamHours { get; set; }
    public int OffsetHours { get; set; }
}
