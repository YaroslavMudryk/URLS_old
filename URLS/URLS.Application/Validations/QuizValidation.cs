using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Quiz;
using URLS.Domain.Models;

namespace URLS.Application.Validations;

public static class QuizValidation
{
    public static bool TryValidate(QuizCreateModel quiz, out string error)
    {
        if (quiz.IsTemplate && quiz.SubjectId != null)
        {
            error = "Template can`t related with subject";
            return false;
        }

        if (quiz.Questions == null)
        {
            error = "Quiz must be contains any questions";
            return false;
        }

        if (quiz.Questions.GroupBy(x => x.Index).Any(g => g.Count() >= 2))
        {
            error = "Indexes can`t be repeating";
            return false;
        }

        foreach (var question in quiz.Questions)
        {
            var isMultipleAnswers = question.IsMultipleAnswers;

            if (!isMultipleAnswers && question.Answers.Count(s => s.IsCorrect) > 1)
            {
                error = "Current question can't support multiple answers";
                return false;
            }
        }



        error = null;
        return true;
    }

    public static Quiz BuildNewQuiz(QuizCreateModel model, IIdentityService identityService)
    {
        var quiz = new Quiz
        {
            Name = model.Name,
            Author = model.Author,
            Config = model.Config,
            IsTemplate = model.IsTemplate,
            SubjectId = model.SubjectId,
            Questions = new List<Question>()
        };

        model.Questions.ForEach(question =>
        {
            var newQuestion = new Question
            {
                Index = question.Index,
                QuestionText = question.QuestionText,
                IsMultipleAnswers = question.IsMultipleAnswers,
                Answers = new List<Answer>()
            };
            newQuestion.PrepareToCreate(identityService);

            if (question.Answers != null)
            {
                question.Answers.ForEach(answer =>
                {
                    var newAnswer = new Answer
                    {
                        IsCorrect = answer.IsCorrect,
                        Response = answer.Response
                    };
                    newAnswer.PrepareToCreate(identityService);
                    newQuestion.Answers.Add(newAnswer);
                });
            }
            quiz.Questions.Add(newQuestion);
        });

        quiz.PrepareToCreate(identityService);

        return quiz;
    }
}
