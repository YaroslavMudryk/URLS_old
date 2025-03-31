using Microsoft.AspNetCore.Mvc;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Quiz;

namespace URLS.Web.Controllers.V1;

[ApiVersion("1.0")]
public class QuizzesController : ApiBaseController
{
    private readonly IQuizService _quizService;
    public QuizzesController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuiz([FromBody] QuizCreateModel quiz)
    {
        return JsonResult(await _quizService.CreateAsync(quiz));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuizes(int offset = 0, int count = 20)
    {
        return JsonResult(await _quizService.GetAllAsync(offset, count));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        return JsonResult(await _quizService.GetByIdAsync(id, true));
    }

    [HttpGet("{id}/statistics")]
    public async Task<IActionResult> GetStatisticsByQuizId(Guid id)
    {
        return JsonResult(await _quizService.GetQuizStatisticsAsync(id));
    }

    [HttpGet("{id}/results")]
    public async Task<IActionResult> GetQuizResults(Guid id, int offset = 0, int count = 10)
    {
        return JsonResult(await _quizService.GetResultsAsync(id, offset, count));
    }

    [HttpGet("{id}/results/{resultId}")]
    public async Task<IActionResult> GetQuizResultById(Guid id, int resultId)
    {
        return JsonResult(await _quizService.GetResultAsync(id, resultId));
    }

    [HttpPost("{id}/start")]
    public async Task<IActionResult> StartQuiz(Guid id)
    {
        return JsonResult(await _quizService.StartQuizAsync(id));
    }

    [HttpPost("{id}/finish/{resultId}")]
    public async Task<IActionResult> FinishQuiz(Guid id, int resultId, QuizAnswerCreateModel model)
    {
        model.QuizId = id;
        return JsonResult(await _quizService.FinishQuizAsync(resultId, model));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuiz(Guid id)
    {
        return JsonResult(await _quizService.DeleteAsync(id));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuiz(Guid id, [FromBody] QuizEditModel model)
    {
        model.Id = id;
        return JsonResult(await _quizService.UpdateAsync(model));
    }
}
