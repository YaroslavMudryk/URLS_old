using AutoMapper;
using Force.DeepCloner;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.Validations;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Quiz;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class QuizService(
    URLSDbContext db,
    IIdentityService identityService,
    IMapper mapper,
    ICommonService commonService) : IQuizService
{
    public async Task<Result<QuizViewModel>> CreateAsync(QuizCreateModel quiz)
    {
        if (!quiz.IsTemplate)
            if (!await commonService.IsExistAsync<Subject>(s => s.Id == quiz.SubjectId))
                return Result<QuizViewModel>.NotFound(typeof(Subject).NotFoundMessage(quiz.SubjectId));

        if (!QuizValidation.TryValidate(quiz, out var error))
            return Result<QuizViewModel>.Error(error);

        var newQuiz = QuizValidation.BuildNewQuiz(quiz, identityService);
        await db.Quizzes.AddAsync(newQuiz);
        await db.SaveChangesAsync();

        var viewModel = mapper.Map<QuizViewModel>(newQuiz);

        return Result<QuizViewModel>.Created(viewModel);
    }

    public async Task<Result<QuizResultViewModel>> FinishQuizAsync(int quizResultId, QuizAnswerCreateModel model)
    {
        var quizResult = await db.QuizResults.AsNoTracking().Include(s => s.Quiz).FirstOrDefaultAsync(s => s.Id == quizResultId);
        if (quizResult == null)
            return Result<QuizResultViewModel>.NotFound(typeof(QuizResult).NotFoundMessage(quizResultId));

        if (quizResult.UserId != identityService.GetUserId())
            return Result<QuizResultViewModel>.Forbiden();

        if (quizResult.EndAt != null)
            return Result<QuizResultViewModel>.Error("Test already pass");

        var maxCountOfAttempts = quizResult.Quiz.Config.MaxAttempts;

        var currentCountOfUserAttempts = await db
            .QuizResults
            .CountAsync(s => s.QuizId == quizResult.QuizId && s.UserId == quizResult.UserId);

        if (maxCountOfAttempts >= 0 && currentCountOfUserAttempts >= maxCountOfAttempts)
            return Result<QuizResultViewModel>.Error("Max attempt of test");
        else
            quizResult.Attempt = currentCountOfUserAttempts + 1;

        var quiz = quizResult.Quiz;
        quizResult.EndAt = DateTime.Now;

        if (quiz.Config.Minutes <= 0)
        {
            quizResult.TimeIsExpired = false;
        }
        else
        {
            var time = DateTime.Now - quizResult.StartAt;
            if (time.TotalMinutes <= quiz.Config.Minutes)
            {
                quizResult.TimeIsExpired = false;
            }
            else
            {
                quizResult.TimeIsExpired = true;
            }
        }

        quizResult.Result = new List<QuestionModel>();

        var questions = await db.Questions.AsNoTracking().Where(s => s.QuizId == model.QuizId).Include(s => s.Answers).ToListAsync();

        if (!TryMapUserAnswersToQuiz(new MapAnswersToQuiz(questions, model.Responses, quizResult, quiz), out var error))//questions, model.Responses, quizResullt, quiz.Config.ShowCorrectAnswers, out var error))
        {
            return Result<QuizResultViewModel>.Error(error);
        }

        quizResult.PrepareToUpdate(identityService);

        db.QuizResults.Update(quizResult);
        await db.SaveChangesAsync();

        var quizResultViewModel = mapper.Map<QuizResultViewModel>(quizResult);

        if (!quizResult.Quiz.Config.ShowResults)
            quizResultViewModel.Result = null;

        return Result<QuizResultViewModel>.SuccessWithData(quizResultViewModel);
    }

    public async Task<Result<QuizViewModel>> GetByIdAsync(Guid id, bool fullTest = false)
    {
        var quiz = await db.Quizzes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        if (quiz == null)
            return Result<QuizViewModel>.NotFound(typeof(Quiz).NotFoundMessage(id));

        if (!CanViewQuiz(quiz))
            return Result<QuizViewModel>.NotFound(typeof(Quiz).NotFoundMessage(id));

        var quizViewModel = mapper.Map<QuizViewModel>(quiz);

        if (fullTest)
        {
            var questions = await db.Questions.AsNoTracking().Where(s => s.QuizId == id).Include(s => s.Answers).OrderBy(s => s.Index).ToListAsync();
            if (quiz.Config.RandomQuestionsAndAnswers)
                quizViewModel.Questions = mapper.Map<List<QuestionViewModel>>(GetRandomQuestions(questions));
            else
                quizViewModel.Questions = mapper.Map<List<QuestionViewModel>>(GetSortingQuestions(questions));
        }

        return Result<QuizViewModel>.SuccessWithData(quizViewModel);
    }

    public async Task<Result<List<QuizViewModel>>> GetBySubjectIdAsync(int subjectId, int offset = 0, int count = 10)
    {
        var quizzes = await db.Quizzes
            .AsNoTracking()
            .Where(s => s.SubjectId == subjectId)
            .Skip(offset).Take(count)
            .ToListAsync();
        var quizzesViewModel = mapper.Map<List<QuizViewModel>>(quizzes);

        var totalCount = await commonService.CountAsync<Quiz>(s => s.SubjectId == subjectId);

        return Result<List<QuizViewModel>>.SuccessList(quizzesViewModel, Meta.FromMeta(totalCount, offset, count));
    }

    public async Task<Result<List<QuizResultViewModel>>> GetResultsAsync(Guid quizId, int offset = 0, int count = 10)
    {
        var request = await commonService.IsExistWithResultsAsync<Quiz>(s => s.Id == quizId);

        if (!request.IsExist)
            return Result<List<QuizResultViewModel>>.NotFound(typeof(Quiz).NotFoundMessage(quizId));

        var quiz = request.Results.First();

        var query = db.QuizResults.AsNoTracking();

        var allResults = quiz.CreatedByUserId == identityService.GetUserId() || identityService.IsAdministrator();

        if (allResults)
            query = query.Where(s => s.QuizId == quizId);
        else
            query = query.Where(s => s.QuizId == quizId && s.CreatedByUserId == identityService.GetUserId());


        query = query.Include(s => s.User)
            .OrderByDescending(s => s.EndAt)
            .Skip(offset).Take(count);

        var results = await query.ToListAsync();
        var resultsViewModel = mapper.Map<List<QuizResultViewModel>>(results);

        resultsViewModel.ForEach(result =>
        {
            result.Result = null;
        });

        var totalCount = await commonService.CountAsync<QuizResult>(s => s.QuizId == quizId);

        return Result<List<QuizResultViewModel>>.SuccessList(resultsViewModel, Meta.FromMeta(totalCount, offset, count));
    }

    public async Task<Result<List<QuizResultViewModel>>> GetUserResultsAsync(int userId, int offset = 0, int count = 10)
    {
        var results = await db.QuizResults
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Include(s => s.Quiz)
            .OrderByDescending(s => s.CreatedAt)
            .Skip(offset).Take(count)
            .ToListAsync();
        var resultsViewModel = mapper.Map<List<QuizResultViewModel>>(results);

        var totalCount = await commonService.CountAsync<QuizResult>(s => s.UserId == userId);

        return Result<List<QuizResultViewModel>>.SuccessList(resultsViewModel, Meta.FromMeta(totalCount, offset, count));
    }

    public async Task<Result<QuizStartedViewModel>> StartQuizAsync(Guid quizId)
    {
        var query = await commonService.IsExistWithResultsAsync<Quiz>(s => s.Id == quizId);
        if (!query.IsExist)
            return Result<QuizStartedViewModel>.NotFound(typeof(Quiz).NotFoundMessage(quizId));

        var currentQuiz = query.Results.First();

        var currentUserId = identityService.GetUserId();

        var newResult = new QuizResult
        {
            Mark = 0,
            UserId = currentUserId,
            QuizId = quizId,
            Result = null,
            Statistics = null,
            StartAt = DateTime.Now,
            EndAt = null,
            TimeIsExpired = false
        };

        var maxAttempts = currentQuiz.Config.MaxAttempts;

        if (maxAttempts >= 1)
        {
            var previewAttempts = await db.QuizResults.CountAsync(s => s.UserId == currentUserId && s.QuizId == quizId);
            if (previewAttempts >= maxAttempts)
                return Result<QuizStartedViewModel>.Error("Your max count of attempts were expired");
            newResult.Attempt = previewAttempts + 1;
        }

        newResult.PrepareToCreate(identityService);
        await db.QuizResults.AddAsync(newResult);
        await db.SaveChangesAsync();

        var quizViewModel = await ReadyQuizToViewAsync(currentQuiz);

        var resultViewModel = new QuizStartedViewModel
        {
            Result = mapper.Map<QuizResultViewModel>(newResult),
            Quiz = quizViewModel
        };

        return Result<QuizStartedViewModel>.SuccessWithData(resultViewModel);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var quizToDelete = await db.Quizzes.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        if (quizToDelete == null)
            return Result<bool>.NotFound(typeof(Quiz).NotFoundMessage(id));

        if (!identityService.IsAdministrator())
            if (quizToDelete.CreatedByUserId != identityService.GetUserId())
                return Result<bool>.Forbiden();

        db.Quizzes.Remove(quizToDelete);
        await db.SaveChangesAsync();
        return Result<bool>.Success();
    }

    public async Task<Result<QuizViewModel>> UpdateAsync(QuizEditModel quizEditModel)
    {
        var quizToUpdate = await db.Quizzes.AsNoTracking().Include(s => s.Questions).ThenInclude(s => s.Answers).FirstOrDefaultAsync(s => s.Id == quizEditModel.Id);
        if (quizToUpdate == null)
            return Result<QuizViewModel>.NotFound(typeof(Quiz).NotFoundMessage(quizEditModel.Id));

        if (!identityService.IsAdministrator())
            if (quizToUpdate.CreatedByUserId != identityService.GetUserId())
                return Result<QuizViewModel>.Forbiden();

        quizToUpdate.Name = quizEditModel.Name;
        quizToUpdate.Description = quizEditModel.Description;
        quizToUpdate.Author = quizEditModel.Author;
        quizToUpdate.Config = quizEditModel.Config;
        quizToUpdate.IsAvalible = quizEditModel.IsAvalible;
        quizToUpdate.From = quizEditModel.From;
        quizToUpdate.To = quizEditModel.To;
        quizToUpdate.IsTemplate = quizEditModel.IsTemplate;

        if (!TryRemoveQuestionsAndAnswers(quizToUpdate, quizEditModel, out var removeError))
        {
            return Result<QuizViewModel>.Error(removeError);
        }

        if (!TryAddQuestionsAndAnswers(quizToUpdate, quizEditModel, out var addError))
        {
            return Result<QuizViewModel>.Error(addError);
        }

        SortQuestions(quizToUpdate, true);

        quizToUpdate.PrepareToUpdate(identityService);
        db.Quizzes.Update(quizToUpdate);
        await db.SaveChangesAsync();

        return Result<QuizViewModel>.SuccessWithData(mapper.Map<QuizViewModel>(quizToUpdate));
    }

    public async Task<Result<List<QuizViewModel>>> GetAllAsync(int offset = 0, int count = 20)
    {
        int totalCount = 0;
        var query = db.Quizzes.AsNoTracking();

        if (!identityService.IsAdministrator())
        {
            query = query.Where(s => s.CreatedByUserId == identityService.GetUserId());
            totalCount = await db.Quizzes.CountAsync(s => s.CreatedByUserId == identityService.GetUserId());
        }
        else
            totalCount = await db.Quizzes.CountAsync();

        query = query.OrderByDescending(s => s.CreatedAt);
        query = query.Skip(offset).Take(count);
        var quizes = await query.ToListAsync();

        return Result<List<QuizViewModel>>.SuccessList(mapper.Map<List<QuizViewModel>>(quizes), Meta.FromMeta(totalCount, offset, count));
    }

    public async Task<Result<QuizResultViewModel>> GetResultAsync(Guid quizId, int quizResultId)
    {
        var request = await commonService.IsExistWithResultsAsync<Quiz>(s => s.Id == quizId);

        if (!request.IsExist)
            return Result<QuizResultViewModel>.NotFound(typeof(Quiz).NotFoundMessage(quizId));

        var quiz = request.Results.First();

        var quizResult = await db.QuizResults.AsNoTracking().FirstOrDefaultAsync(s => s.Id == quizResultId);

        if (quizResult == null)
            return Result<QuizResultViewModel>.NotFound(typeof(QuizResult).NotFoundMessage(quizResultId));

        if (!identityService.IsAdministrator())
            if (quizResult.CreatedByUserId != identityService.GetUserId())
                if (quiz.CreatedByUserId != identityService.GetUserId())
                    return Result<QuizResultViewModel>.Forbiden();

        var quizResultViewModel = mapper.Map<QuizResultViewModel>(quizResult);

        if (!identityService.IsAdministrator())
            if (quizResult.UserId == identityService.GetUserId() && !quiz.Config.ShowResults)
                quizResultViewModel.Result = null;

        return Result<QuizResultViewModel>.SuccessWithData(quizResultViewModel);
    }

    public async Task<Result<QuizStatisticsViewModel>> GetQuizStatisticsAsync(Guid quizId)
    {
        var quizResponse = await commonService.IsExistWithResultsAsync<Quiz>(s => s.Id == quizId);
        if (!quizResponse.IsExist)
            return Result<QuizStatisticsViewModel>.NotFound(typeof(Quiz).NotFoundMessage(quizId));


        var quizResults = await db.QuizResults.AsNoTracking().Where(s => s.QuizId == quizId).ToListAsync();

        if (quizResults == null || quizResults.Count == 0)
            return Result<QuizStatisticsViewModel>.Success();

        var statistics = new QuizStatisticsViewModel
        {
            First = quizResults.MinBy(s => s.EndAt).EndAt,
            Last = quizResults.MaxBy(s => s.EndAt).EndAt,
            TotalCount = quizResults.Count,
            AverageTimeInSeconds = quizResults.GetAverageTimeInSeconds()
        };

        return Result<QuizStatisticsViewModel>.SuccessWithData(statistics);
    }

    #region Private
    private bool TryMapUserAnswersToQuiz(MapAnswersToQuiz answersToQuiz, out string error)//List<Question> questions, List<QuizAnswerResponse> quizResponse, QuizResult result, bool showCorrectAnswer, out string error)
    {
        if (!TryCheckQuestions(answersToQuiz.Questions, answersToQuiz.QuizResponse, out var errorQuestion))
        {
            error = errorQuestion;
            return false;
        }

        if (!TryCheckAnswers(answersToQuiz.Questions, answersToQuiz.QuizResponse, out var errorAnswers))
        {
            error = errorAnswers;
            return false;
        }


        var maxMark = Math.Round(answersToQuiz.Quiz.Config.MarkPerQuiz, 2);
        var markPerTrueAnswer = maxMark / answersToQuiz.Questions.Count;

        double markResult = 0;

        foreach (var question in answersToQuiz.Questions)
        {
            var resposneModel = answersToQuiz.QuizResponse.FirstOrDefault(s => s.QuestionId == question.Id);

            var questionModel = new QuestionModel
            {
                Id = question.Id,
                QuestionText = question.QuestionText,
                Answers = new List<AnswerModel>()
            };

            var perOneTrueAnswer = markPerTrueAnswer / question.Answers.Count();

            foreach (var answer in question.Answers)
            {
                var answerModel = new AnswerModel
                {
                    Id = answer.Id,
                    Response = answer.Response,
                    IsCorrect = answersToQuiz.Quiz.Config.ShowCorrectAnswers ? answer.IsCorrect : null,
                    IsChoice = resposneModel?.AnswerIds?.Contains(answer.Id)
                };

                answerModel.ChoiceAt = answerModel.IsChoice == true ? resposneModel?.ChoiceAt : null;
                if (answerModel.IsCorrectAnswer())
                    if (question.IsMultipleAnswers)
                    {
                        markResult += perOneTrueAnswer;
                    }
                    else
                    {
                        markResult += markPerTrueAnswer;
                    }

                questionModel.Answers.Add(answerModel);
            }
            answersToQuiz.Result.Result.Add(questionModel);
        }
        answersToQuiz.Result.Mark = Math.Round(markResult, 2);


        error = null;
        return true;
    }

    private bool CanViewQuiz(Quiz quiz)
    {
        if (identityService.IsAdministrator())
            return true;
        if (quiz.CreatedByUserId == identityService.GetUserId())
            return true;
        if (!quiz.IsAvalible && quiz.CreatedByUserId != identityService.GetUserId())
            return false;
        return false;
    }

    private bool TryRemoveQuestionsAndAnswers(Quiz currentQuiz, QuizEditModel quizEditModel, out string error)
    {
        var questionIds = new List<int>();
        var answerIds = new List<long>();
        foreach (var question in currentQuiz.Questions)
        {
            var questionUpdateModel = quizEditModel.Questions.FirstOrDefault(x => x.Id == question.Id);
            if (questionUpdateModel != null)
            {
                question.Index = questionUpdateModel.Index;
                question.IsMultipleAnswers = questionUpdateModel.IsMultipleAnswers;
                question.QuestionText = questionUpdateModel.QuestionText;
                question.PrepareToUpdate(identityService);

                foreach (var answer in question.Answers)
                {
                    var answerUpdateModel = questionUpdateModel.Answers.FirstOrDefault(s => s.Id == answer.Id);
                    if (answerUpdateModel != null)
                    {
                        answer.IsCorrect = answerUpdateModel.IsCorrect;
                        answer.Response = answerUpdateModel.Response;
                        answer.PrepareToUpdate(identityService);
                    }
                    else
                    {
                        db.Answers.Remove(answer);
                        answerIds.Add(answer.Id);
                    }
                }
            }
            else
            {
                db.Questions.Remove(question);
                questionIds.Add(question.Id);
            }
        }

        SyncData(quizEditModel, questionIds, answerIds);

        error = null;
        return true;
    }

    private bool TryAddQuestionsAndAnswers(Quiz currentQuiz, QuizEditModel quizEditModel, out string error)
    {
        if (quizEditModel.Questions.GroupBy(x => x.Index).Any(g => g.Count() > 1))
        {
            error = "Indexes can`t be repeating";
            return false;
        }

        var userId = identityService.GetUserId();

        foreach (var question in quizEditModel.Questions.Where(s => !s.Id.HasValue))
        {
            var newQuestion = new Question
            {
                QuestionText = question.QuestionText,
                IsMultipleAnswers = question.IsMultipleAnswers,
                Index = question.Index,
                CreatedByUserId = userId,
                Answers = new List<Answer>()
            };

            foreach (var answer in question.Answers.Where(s => !s.Id.HasValue))
            {
                var newAnswer = new Answer
                {
                    IsCorrect = answer.IsCorrect,
                    Response = answer.Response
                };
                newAnswer.PrepareToCreate(identityService);
                newQuestion.Answers.Add(newAnswer);
            }

            newQuestion.PrepareToCreate(identityService);
            currentQuiz.Questions.Add(newQuestion);
        }

        error = null;
        return true;
    }

    private void SortQuestions(Quiz currentQuiz, bool fixIndex = false)
    {
        var copyQuestions = currentQuiz.Questions.DeepClone();

        currentQuiz.Questions.Clear();

        if (fixIndex)
        {
            var index = 1;
            foreach (var question in copyQuestions.OrderBy(s => s.Index))
            {
                question.Index = index++;
                currentQuiz.Questions.Add(question);
            }
        }
        else
        {
            currentQuiz.Questions = copyQuestions.OrderBy(s => s.Index).ToList();
        }
    }

    private void SyncData(QuizEditModel quiz, List<int> questionIds, List<long> answerIds)
    {
        foreach (var question in quiz.Questions)
        {
            question.Answers.Where(s => s.Id.HasValue).ToList().RemoveAll(s => answerIds.Contains(s.Id.Value));
        }
        quiz.Questions.Where(s => s.Id.HasValue).ToList().RemoveAll(s => questionIds.Contains(s.Id.Value));
    }

    private bool TryCheckQuestions(List<Question> questions, List<QuizAnswerResponse> quizResponse, out string error)
    {
        var diffQuestions = quizResponse.Select(s => s.QuestionId).Except(questions.Select(s => s.Id));

        if (diffQuestions != null && diffQuestions.Count() > 0)
        {
            if (diffQuestions.Count() == 1)
                error = $"Питання з Id {diffQuestions.First()} не існує";
            else
                error = $"Питань з Id ({string.Join(",", diffQuestions)}) не існує";
            return false;
        }
        error = null;
        return true;
    }

    private bool TryCheckAnswers(List<Question> questions, List<QuizAnswerResponse> quizResponse, out string error)
    {
        var originalAnswerdIds = new List<long>();

        questions.ForEach(question =>
        {
            question.Answers.ForEach(answer =>
            {
                originalAnswerdIds.Add(answer.Id);
            });
        });

        var answerdIds = new List<long>();

        quizResponse.ForEach(response =>
        {
            response.AnswerIds.ToList().ForEach(answer =>
            {
                answerdIds.Add(answer);
            });
        });

        var diffAnswers = answerdIds.Except(originalAnswerdIds);

        if (diffAnswers != null && diffAnswers.Count() > 0)
        {
            if (diffAnswers.Count() == 1)
                error = $"Відповіді з Id {diffAnswers.First()} не існує";
            else
                error = $"Відповідей з Id ({string.Join(",", diffAnswers)}) не існує";
            return false;
        }

        foreach (var response in quizResponse)
        {
            var question = questions.FirstOrDefault(s => s.Id == response.QuestionId);
            var answerDiff = response.AnswerIds.Except(question.Answers.Select(s => s.Id));
            if (answerDiff != null && answerDiff.Count() > 0)
            {
                error = $"Відповіді з Id ({string.Join(",", answerDiff)}) для питання з Id ({question.Id}) не знайдено";
                return false;
            }
        }
        error = null;
        return true;
    }

    private async Task<QuizViewModel> ReadyQuizToViewAsync(Quiz quiz)
    {
        var quizViewModel = mapper.Map<QuizViewModel>(quiz);

        var questions = await db.Questions.AsNoTracking().Where(s => s.QuizId == quiz.Id).Include(s => s.Answers).ToListAsync();

        if (quiz.Config.RandomQuestionsAndAnswers)
            quizViewModel.Questions = GetRandomQuestions(questions);
        else
            quizViewModel.Questions = GetSortingQuestions(questions);

        return quizViewModel;
    }

    private List<QuestionViewModel> GetSortingQuestions(List<Question> questions)
    {
        var sortingQuestions = new List<QuestionViewModel>();

        if (questions == null || questions.Count == 0)
            return sortingQuestions;

        int questionNumber = 1;
        foreach (var question in questions.OrderBy(s => s.Index))
        {
            var questionViewModel = new QuestionViewModel
            {
                Id = question.Id,
                Index = question.Index,
                CreatedAt = question.CreatedAt,
                QuestionText = question.QuestionText.GetNumericQuestion(questionNumber),
                Answers = new List<AnswerViewModel>()
            };
            if (question.Answers != null && question.Answers.Count > 0)
            {
                int answerNumber = 1;
                foreach (var answer in question.Answers)
                {
                    questionViewModel.Answers.Add(new AnswerViewModel
                    {
                        Id = answer.Id,
                        Response = answer.Response.GetNumericAnswer(answerNumber)
                    });
                    answerNumber++;
                }
            }
            sortingQuestions.Add(questionViewModel);
            questionNumber++;
        }

        return sortingQuestions;
    }

    private List<QuestionViewModel> GetRandomQuestions(List<Question> questions)
    {
        var randomQuestions = new List<QuestionViewModel>();

        if (questions == null || questions.Count == 0)
            return randomQuestions;

        var questionIds = questions.Select(x => x.Id).ToList();

        for (int i = 1; i <= questions.Count; i++)
        {
            var randomQuestionId = questionIds[new Random().Next(0, questionIds.Count)];
            var question = questions.FirstOrDefault(s => s.Id == randomQuestionId);
            var questionViewModel = new QuestionViewModel
            {
                Id = question.Id,
                CreatedAt = question.CreatedAt,
                Index = question.Index,
                QuestionText = question.QuestionText.GetNumericQuestion(i),
                Answers = new List<AnswerViewModel>()
            };

            if (question.Answers != null && question.Answers.Count > 0)
            {
                var answersIds = question.Answers.Select(s => s.Id).ToList();
                for (int j = 1; j <= question.Answers.Count; j++)
                {
                    var randomAnswerId = answersIds[new Random().Next(0, answersIds.Count)];
                    var answer = question.Answers.FirstOrDefault(s => s.Id == randomAnswerId);
                    var answerViewModel = new AnswerViewModel
                    {
                        Id = answer.Id,
                        Response = answer.Response.GetNumericAnswer(j),
                    };
                    answersIds.Remove(answer.Id);
                    questionViewModel.Answers.Add(answerViewModel);
                }
            }
            randomQuestions.Add(questionViewModel);
            questionIds.Remove(question.Id);
        }
        return randomQuestions;
    }
    #endregion
}
