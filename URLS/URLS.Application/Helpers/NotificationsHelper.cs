using Extensions.Converters;
using Extensions.DeviceDetector.Models;
using System.Text;
using URLS.Application.ViewModels.Identity;
using URLS.Constants;
using URLS.Domain.Models;

namespace URLS.Application.Helpers;

public static class NotificationsHelper
{
    private static Notification Plug => new Notification();

    public static Notification GetWelcomeNotification()
    {
        return new Notification
        {
            Title = "Вітаємо у ДУТ СДН!",
            Content = "Ласкаво просимо у систему ДУТ СДН. Сподіваємося вам сподобається досвід користування данною системою!",
            ImageUrl = "https://previews.123rf.com/images/foxysgraphic/foxysgraphic1907/foxysgraphic190700061/129432470-welcome-banner-speech-bubble-poster-concept-geometric-memphis-style-with-text-welcome-icon-balloon-w.jpg",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.Welcome,
            IsImportant = false,
            IsRead = false,
            ReadAt = null,
        };
    }

    public static Notification GetAcceptInGroupNotification(Group group)
    {
        return new Notification
        {
            Title = $"Вас доєднано до групи {group.Name}",
            Content = $"Ви були доєднані до групи {group.Name}. Бажаємо Вам успішного навчання!",
            ImageUrl = "https://cdn1.iconfinder.com/data/icons/color-bold-style/21/34-512.png",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.AcceptedInGroup,
            IsImportant = false,
            IsRead = false,
            ReadAt = null,
        };
    }

    public static Notification GetLoginByPasswordNotification(Session session)
    {
        return new Notification
        {
            Title = "Новий вхід",
            Content = $"Увага! Щойно було виконано вхід на ваш акаунт з присторою {GetDeviceInfo(session.Client)} в {session.Location.Country}, {session.Location.City} [{session.Location.IP}]",
            ImageUrl = "https://cdn-icons-png.flaticon.com/512/152/152533.png",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.NewLogin,
            IsImportant = true,
            IsRead = false,
            ReadAt = null
        };
    }

    public static Notification GetLoginBySocialNotification(Session session)
    {
        return new Notification
        {
            Title = "Новий вхід",
            Content = $"Увага! Щойно було виконано вхід на ваш акаунт через {session.Type} з присторою {GetDeviceInfo(session.Client)} в {session.Location.Country}, {session.Location.City} [{session.Location.IP}]",
            ImageUrl = "",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.NewLogin,
            IsImportant = true,
            IsRead = false,
            ReadAt = null
        };
    }

    public static Notification GetLoginAttemptNotification(LoginCreateModel loginModel, string ip)
    {
        return new Notification
        {
            Title = "Спроба входу",
            Content = $"Увага! Щойно було виконано спроба входу на ваш акаунт з паролем ({loginModel.Password}) [{ip}]",
            ImageUrl = "https://icon-library.com/images/hack-icon/hack-icon-19.jpg",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.LoginAttempt,
            IsImportant = true,
            IsRead = false,
            ReadAt = null
        };
    }

    public static Notification GetChangePasswordNotification()
    {
        return new Notification
        {
            Title = "Пароль було змінено",
            Content = "Увага! Ваш пароль було успішно змінено",
            ImageUrl = "https://thumbs.dreamstime.com/b/lock-login-password-safe-security-icon-vector-illustration-flat-design-lock-login-password-safe-security-icon-vector-illustration-131742100.jpg",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.ChangePassword,
            IsImportant = true,
            IsRead = false,
            ReadAt = null,
        };
    }

    public static Notification GetLogoutNotification(Session session)
    {
        return new Notification
        {
            Title = "Вихід",
            Content = $"Увага! Щойно було виконано вихід на вашому акаунті на пристрої {GetDeviceInfo(session.Client)}",
            ImageUrl = "https://thumbs.dreamstime.com/b/lock-login-password-safe-security-icon-vector-illustration-flat-design-lock-login-password-safe-security-icon-vector-illustration-131742100.jpg",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.Logout,
            IsImportant = true,
            IsRead = false,
            ReadAt = null,
        };
    }

    public static Notification GetAllLogoutNotification()
    {
        return new Notification
        {
            Title = "Вихід",
            Content = $"Увага! Щойно було виконано вихід з вашого акаунту на всіх пристроях",
            ImageUrl = "https://thumbs.dreamstime.com/b/lock-login-password-safe-security-icon-vector-illustration-flat-design-lock-login-password-safe-security-icon-vector-illustration-131742100.jpg",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.Logout,
            IsImportant = true,
            IsRead = false,
            ReadAt = null,
        };
    }

    public static Notification GetLockedNotification()
    {
        return new Notification
        {
            Title = "Блокування",
            Content = $"Увага! Через велику кількість невдалих спроб увійти ваш акаунт був заблокован на 1 годину, спробуйте пізніше",
            ImageUrl = "https://media.istockphoto.com/vectors/lock-icon-vector-id936681148?k=20&m=936681148&s=612x612&w=0&h=j6fxNWrJ09iE7khUsDWetKn_PwWydgIS0yFJBEonGow=",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.Locked,
            IsImportant = true,
            IsRead = false,
            ReadAt = null,
        };
    }

    public static Notification GetChangePermissionNotification()
    {
        return new Notification
        {
            Title = "Зміна дозволів",
            Content = $"Увага! Через зміну дозволів до вашої ролі робота деякого функціоналу буде відбуватися не коректно. Будь-ласка перезайдіть на власний акаунт на всіх пристроях!",
            ImageUrl = "https://www.sop.com.ua/images/icons/123.png",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.PermissionChanged,
            IsImportant = true,
            IsRead = false,
            ReadAt = null,
        };
    }

    public static Notification GetChangeRoleNotification()
    {
        return new Notification
        {
            Title = "Зміна ролі",
            Content = $"Увага! Через зміну ваших ролей подальша робота може бути некоректною. Будь-ласка за можливостю перезайдіть на всіх пристроях",
            ImageUrl = "https://cdn-icons-png.flaticon.com/512/272/272354.png",
            CreatedAt = DateTime.Now,
            CreatedBy = Defaults.CreatedBy,
            CreatedFromIP = "::1",
            Type = NotificationType.RolesChanged,
            IsImportant = true,
            IsRead = false,
            ReadAt = null,
        };
    }

    private static string GetDeviceInfo(ClientInfo clientInfo)
    {
        StringBuilder result = new StringBuilder();
        if (clientInfo.Device.Brand.IsNullOrEmpty() && clientInfo.Device.Model.IsNullOrEmpty())
        {
            result.Append(clientInfo.Device.Type);
            result.Append(" ");
            result.Append($"({clientInfo.OS.Name} {clientInfo.OS.Version})");
        }
        else
        {
            result.Append(clientInfo.Device.Brand);
            result.Append(" ");
            result.Append(clientInfo.Device.Model);
            result.Append($"({clientInfo.OS.Name} {clientInfo.OS.Version})");
        }
        return result.ToString();
    }

    public static Notification GetNewPostNotification()
    {
        return Plug;
    }
}
