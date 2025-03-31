using URLS.Application.ViewModels.Group;
using URLS.Application.ViewModels.Group.GroupMember;
using URLS.Application.ViewModels.Group.GroupRole;
using URLS.Domain.Models;

namespace URLS.Application.Extensions;

public static class UserGroupExtensions
{
    public static GroupMemberViewModel MapToView(this UserGroup userGroup, bool withPermissions = true)
    {
        if (userGroup == null)
            return null;
        return new GroupMemberViewModel
        {
            Id = userGroup.Id,
            CreatedAt = userGroup.CreatedAt,
            IsAdmin = userGroup.IsAdmin,
            Status = userGroup.Status,
            Title = userGroup.Title,
            User = userGroup.User != null ?
            new ViewModels.User.UserViewModel
            {
                Id = userGroup.User.Id,
                FirstName = userGroup.User.FirstName,
                LastName = userGroup.User.LastName,
                ContactEmail = userGroup.User.ContactEmail,
                ContactPhone = userGroup.User.ContactPhone,
                FullName = $"{userGroup.User.LastName} {userGroup.User.FirstName}",
                Image = userGroup.User.Image,
                MiddleName = userGroup.User.MiddleName,
                UserName = userGroup.User.UserName,
                JoinAt = userGroup.User.JoinAt
            } : null,
            UserGroupRole = userGroup.UserGroupRole != null ?
            new UserGroupRoleViewModel
            {
                Id = userGroup.UserGroupRole.Id,
                CreatedAt = userGroup.UserGroupRole.CreatedAt,
                Name = userGroup.UserGroupRole.Name,
                NameEng = userGroup.UserGroupRole.NameEng,
                Color = userGroup.UserGroupRole.Color,
                Description = userGroup.UserGroupRole.Description,
                DescriptionEng = userGroup.UserGroupRole.DescriptionEng,
                Permissions = withPermissions ? userGroup.UserGroupRole.Permissions : null
            } : null,
            Group = userGroup.Group != null ?
            new GroupViewModel
            {
                Id = userGroup.Group.Id,
                Name = userGroup.Group.Name,
                Course = userGroup.Group.Course,
                CreatedAt = userGroup.Group.CreatedAt,
                Image = userGroup.Group.Image,
                StartStudy = userGroup.Group.StartStudy
            } : null
        };
    }

    public static List<GroupMemberViewModel> MapToViews(this List<UserGroup> userGroups, bool withPermissions = true)
    {
        if (userGroups == null || userGroups.Count == 0)
            return null;

        return userGroups
            .Select(x => x.MapToView(withPermissions))
            .OrderBy(s => s.User.LastName)
            .ToList();
    }

    public static string GetStatusName(this UserGroupStatus groupStatus)
    {
        if (groupStatus == UserGroupStatus.New)
            return "Новачок";
        if (groupStatus == UserGroupStatus.Member)
            return "Учасник";
        if (groupStatus == UserGroupStatus.Gona)
            return "Пішов";
        return "Невідомо";
    }

    public static string GetLessonType(this LessonType lessonType)
    {
        if (lessonType == LessonType.Lecture)
            return "Лекція";
        if (lessonType == LessonType.Laboratory)
            return "Лабораторна";
        if (lessonType == LessonType.Practical)
            return "Практична";
        if (lessonType == LessonType.Individual)
            return "Самостійна";
        if (lessonType == LessonType.Offset)
            return "Практика";
        if (lessonType == LessonType.Exam)
            return "Екзамен";
        return "Інший";
    }
}
