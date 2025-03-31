using AutoMapper;
using Microsoft.EntityFrameworkCore;
using URLS.Application.Extensions;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels;
using URLS.Application.ViewModels.Notification;
using URLS.Constants.APIResponse;
using URLS.Constants.Extensions;
using URLS.Domain.Models;
using URLS.Infrastructure.Data.Context;

namespace URLS.Application.Services.Implementations;

public class NotificationService(
    URLSDbContext db,
    IMapper mapper,
    IIdentityService identityService,
    ICommonService commonService) : INotificationService
{
    public async Task<Result<NotificationViewModel>> GetNotificationByIdAsync(long notifyId)
    {
        var notification = await db.Notifications.AsNoTracking().FirstOrDefaultAsync(x => x.Id == notifyId);
        if (notification == null)
            return Result<NotificationViewModel>.NotFound(typeof(Notification).NotFoundMessage(notifyId));

        if (!identityService.IsAdministrator())
            if (notification.UserId != identityService.GetUserId())
                return Result<NotificationViewModel>.Forbiden();

        return Result<NotificationViewModel>.SuccessWithData(mapper.Map<NotificationViewModel>(notification));
    }

    public async Task<Result<List<NotificationViewModel>>> GetUserNotificationsAsync(int userId, int offset, int count)
    {
        if (!identityService.IsAdministrator())
            if (userId != identityService.GetUserId())
                return Result<List<NotificationViewModel>>.Forbiden();

        var notifications = await db.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Skip(offset).Take(count)
            .ToListAsync();

        var notificationViewModels = mapper.Map<List<NotificationViewModel>>(notifications);

        var totalCount = await commonService.CountAsync<Notification>(x => x.UserId == userId);

        return Result<List<NotificationViewModel>>.SuccessList(notificationViewModels, Meta.FromMeta(totalCount, offset, count));
    }

    public async Task<Result<bool>> ReadAllUserNotificationsAsync(int userId)
    {
        if (!await commonService.IsExistAsync<User>(s => s.Id == userId))
            return Result<bool>.NotFound(typeof(User).NotFoundMessage(userId));

        var notifications = await db.Notifications.AsNoTracking().Where(s => s.UserId == userId).ToListAsync();
        if (notifications == null || notifications.Count == 0)
            return Result<bool>.SuccessWithData(true);

        var now = DateTime.Now;

        notifications.ForEach(not =>
        {
            not.ReadAt = now;
            not.IsRead = true;
            not.PrepareToUpdate(identityService);
        });

        db.Notifications.UpdateRange(notifications);
        await db.SaveChangesAsync();
        return Result<bool>.SuccessWithData(true);
    }

    public async Task<Result<NotificationViewModel>> ReadNotificationAsync(long notifyId)
    {
        var notification = await db.Notifications.FirstOrDefaultAsync(x => x.Id == notifyId);

        if (!identityService.IsAdministrator())
            if (notification.UserId != identityService.GetUserId())
                return Result<NotificationViewModel>.Forbiden();

        if (notification.IsRead)
            return Result<NotificationViewModel>.Error("Notification is already reading");

        notification.IsRead = true;
        notification.ReadAt = DateTime.Now;
        notification.PrepareToUpdate(identityService);

        db.Notifications.Update(notification);
        await db.SaveChangesAsync();

        return Result<NotificationViewModel>.SuccessWithData(mapper.Map<NotificationViewModel>(notification));
    }

    public async Task<Result<bool>> SendNotifyToUserAsync(Notification notification, int userId)
    {
        notification.UserId = userId;
        notification.PrepareToCreate(identityService);
        await db.Notifications.AddAsync(notification);
        await db.SaveChangesAsync();
        return Result<bool>.SuccessWithData(true);
    }

    public async Task<Result<bool>> SendNotifyToUsersAsync(Notification notification, IEnumerable<int> userIds)
    {
        var notifications = new List<Notification>();

        foreach (int userId in userIds)
        {
            var notify = new Notification
            {
                Title = notification.Title,
                Content = notification.Content,
                ImageUrl = notification.ImageUrl,
                IsImportant = notification.IsImportant,
                Type = notification.Type,
                UserId = userId
            };
            notify.PrepareToCreate();
            notifications.Add(notify);
        }
        await db.Notifications.AddRangeAsync(notifications);
        await db.SaveChangesAsync();
        return Result<bool>.SuccessWithData(true);
    }
}
