namespace URLS.Application.ViewModels.Quiz;

public class QuizStatisticsViewModel
{
    public DateTime? First { get; set; }
    public DateTime? Last { get; set; }
    public int TotalCount { get; set; }
    public int AverageTimeInSeconds { get; set; }
}
