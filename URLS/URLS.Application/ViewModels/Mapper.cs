using AutoMapper;
using System.Text;
using URLS.Application.Helpers;
using URLS.Application.ViewModels.Apps;
using URLS.Application.ViewModels.Diploma;
using URLS.Application.ViewModels.Faculty;
using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Application.ViewModels.Group.GroupRole;
using URLS.Application.ViewModels.Identity;
using URLS.Application.ViewModels.Lesson;
using URLS.Application.ViewModels.Notification;
using URLS.Application.ViewModels.Post;
using URLS.Application.ViewModels.Post.Comment;
using URLS.Application.ViewModels.Quiz;
using URLS.Application.ViewModels.Reaction;
using URLS.Application.ViewModels.Report;
using URLS.Application.ViewModels.RoleClaim;
using URLS.Application.ViewModels.Session;
using URLS.Application.ViewModels.Setting;
using URLS.Application.ViewModels.Specialty;
using URLS.Application.ViewModels.Subject;
using URLS.Application.ViewModels.Timetable;
using URLS.Application.ViewModels.University;
using URLS.Application.ViewModels.User;
using URLS.Application.ViewModels.User.UserInfo;

namespace URLS.Application.ViewModels;

public class Mapper : Profile
{
    public Mapper()
    {
        CreateMap<Domain.Models.University, UniversityViewModel>();
        CreateMap<Domain.Models.University, UniversityCreateModel>().ReverseMap();
        CreateMap<Domain.Models.University, UniversityEditModel>().ReverseMap();

        CreateMap<Domain.Models.Faculty, FacultyViewModel>();
        CreateMap<Domain.Models.Faculty, FacultyCreateModel>().ReverseMap();
        CreateMap<Domain.Models.Faculty, FacultyEditModel>().ReverseMap();

        CreateMap<Domain.Models.Specialty, SpecialtyViewModel>();
        CreateMap<Domain.Models.Specialty, SpecialtyCreateModel>().ReverseMap();
        CreateMap<Domain.Models.Specialty, SpecialtyEditModel>().ReverseMap();


        CreateMap<Domain.Models.Group, GroupViewModel>()
            .ForMember(x => x.Name, s => s.MapFrom(x => $"{x.Name} ({x.StartStudy.Year})"))
            .ForMember(x => x.StudyingIsOver, s => s.MapFrom(x => x.EndStudy < DateTime.Now ? true : false));
        CreateMap<Domain.Models.Group, GroupShortViewModel>()
            .ForMember(x => x.Name, s => s.MapFrom(x => $"{x.Name} ({x.StartStudy.Year})"))
            .ForMember(x => x.StudyingIsOver, s => s.MapFrom(x => x.EndStudy < DateTime.Now ? true : false));

        CreateMap<Domain.Models.GroupInvite, GroupInviteViewModel>();

        CreateMap<Domain.Models.UserGroupRole, UserGroupRoleViewModel>().ReverseMap();


        CreateMap<Domain.Models.User, UserViewModel>()
            .ForMember(x => x.FullName, s => s.MapFrom(s => BuildFullName(s)));
        CreateMap<Domain.Models.User, UserShortViewModel>().ReverseMap();

        CreateMap<Domain.Models.App, AppViewModel>().ReverseMap();
        CreateMap<Domain.Models.App, AppEditModel>().ReverseMap();
        CreateMap<Domain.Models.App, AppCreateModel>().ReverseMap();

        CreateMap<Domain.Models.Session, SessionViewModel>().ReverseMap();

        CreateMap<Domain.Models.Notification, NotificationViewModel>().ReverseMap();

        CreateMap<Domain.Models.Diploma, DiplomaViewModel>().ReverseMap();

        CreateMap<Domain.Models.Comment, CommentViewModel>().ReverseMap();
        CreateMap<Domain.Models.Post, PostViewModel>()
            .ForMember(s => s.AvailableReactions, s => s.MapFrom(s => s.IsAvailableReactions ? ReactionHelper.MapAvailableReactions(s.AvailableReactionIds) : null))
            .ReverseMap();

        CreateMap<Domain.Models.Subject, SubjectViewModel>().ReverseMap();
        CreateMap<Domain.Models.Lesson, LessonViewModel>().ReverseMap();

        CreateMap<Domain.Models.Claim, ClaimViewModel>().ReverseMap();
        CreateMap<Domain.Models.Role, RoleViewModel>().ReverseMap();

        CreateMap<Domain.Models.Setting, SettingViewModel>().ReverseMap();
        CreateMap<Domain.Models.User, UserFullViewModel>()
            .ForMember(x => x.FullName, s => s.MapFrom(s => BuildFullName(s))).ReverseMap();

        CreateMap<Domain.Models.Timetable, TimetableViewModel>().ReverseMap();

        CreateMap<Domain.Models.Report, ReportViewModel>().ReverseMap();

        CreateMap<Domain.Models.Quiz, QuizViewModel>().ReverseMap();
        CreateMap<Domain.Models.Question, QuestionViewModel>().ReverseMap();
        CreateMap<Domain.Models.Answer, AnswerViewModel>().ReverseMap();
        CreateMap<Domain.Models.QuizResult, QuizResultViewModel>().ReverseMap();

        CreateMap<Domain.Models.Reaction, ReactionViewModel>()
            .ForMember(s => s.Reaction, s => s.MapFrom(s => ReactionHelper.GetReactionFromId(s.ReactionTypeId)))
            .ReverseMap();

        CreateMap<Domain.Models.UserLogin, SocialViewModel>()
            .ForMember(s => s.LinkedAt, s => s.MapFrom(s => s.CreatedAt))
            .ForMember(s => s.Provider, s => s.MapFrom(s => s.ExternalProvider));

        CreateMap<Domain.Models.UserSpecialty, SpecialtyTeacherViewModel>()
            .ForMember(s => s.Teacher, s => s.MapFrom(s => s.User));

        CreateMap<Domain.Models.UserGroup, GroupMemberViewModel>().ReverseMap();
    }

    private string BuildFullName(Domain.Models.User user)
    {
        var sb = new StringBuilder();
        sb.Append(user.LastName);
        sb.Append(" ");
        sb.Append(user.FirstName);
        if (!string.IsNullOrWhiteSpace(user.MiddleName))
        {
            sb.Append(" ");
            sb.Append(user.MiddleName);
        }
        return sb.ToString();
    }
}
