﻿using Extensions.DeviceDetector;
using Extensions.DeviceDetector.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using URLS.Application.Services.Interfaces;
using URLS.Application.ViewModels.Identity;
using URLS.Application.ViewModels.User;
namespace URLS.Web.Controllers.V1
{
    [ApiVersion("1.0")]
    public class AccountController : ApiBaseController
    {
        private readonly Application.Services.Interfaces.IAuthenticationService _authenticationService;
        private readonly ISessionService _sessionService;
        private readonly IIdentityService _identityService;
        public AccountController(Application.Services.Interfaces.IAuthenticationService authenticationService,
            ISessionService sessionService,
            IIdentityService identityService)
        {
            _authenticationService = authenticationService;
            _sessionService = sessionService;
            _identityService = identityService;
        }

        #region Identity

        [HttpGet("claims")]
        public IActionResult GetClaims()
        {
            var data = User.Claims.Select(x => new
            {
                Type = x.Type,
                Value = x.Value
            }).ToList();
            return Ok(data);
        }

        [HttpGet("socials")]
        public async Task<IActionResult> GetUserSocials()
        {
            return JsonResult(await _authenticationService.GetUserLoginsAsync(_identityService.GetUserId()));
        }

        [HttpDelete("socials/{id}")]
        public async Task<IActionResult> RemoveSocial(int id)
        {
            return JsonResult(await _authenticationService.UnlinkSocialAsync(id));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCreateModel model)
        {
            return JsonResult(await _authenticationService.LoginByPasswordAsync(model));
        }

        [HttpGet("social/{scheme}")]
        [AllowAnonymous]
        public async Task SocialAuth(string scheme)
        {
            var auth = await Request.HttpContext.AuthenticateAsync(scheme);

            if (!auth.Succeeded
                || auth?.Principal == null
                || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
                || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
            {
                await Request.HttpContext.ChallengeAsync(scheme);
            }
            else
            {
                var result = await _authenticationService.LoginBySocialAsync(auth, scheme);

                var qs = new Dictionary<string, string>();

                if (result.IsSuccess)
                {
                    qs.Add("access_token", result.Data.Token);
                    qs.Add("refresh_token", string.Empty);
                    qs.Add("jwt_token_expires", result.Data.ExpiredAt.ToLongDateString());
                }
                else
                {
                    qs.Add("access_token", string.Empty);
                    qs.Add("refresh_token", string.Empty);
                    qs.Add("jwt_token_expires", 0.ToString());
                    qs.Add("error", result.ErrorMessage);
                }

                var url = "urlsapp" + "://#" + string.Join(
                    "&",
                    qs.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
                    .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

                Request.HttpContext.Response.Redirect(url);
            }
        }

        [HttpPost("registration")]
        [AllowAnonymous]
        public async Task<IActionResult> Registration([FromBody] RegisterViewModel model)
        {
            return JsonResult(await _authenticationService.RegisterAsync(model));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            return JsonResult(await _authenticationService.LogoutAsync());
        }

        [HttpPost("config")]
        public async Task<IActionResult> ConfigUser([FromBody] BlockUserModel model)
        {
            return JsonResult(await _authenticationService.BlockUserConfigAsync(model));
        }

        #endregion

        #region Sessions

        [HttpGet("sessions")]
        public async Task<IActionResult> GetActiveSessions(int userId = default, int q = 0, int offset = 0, int limit = 20)
        {
            if (userId == default || userId < 1)
                userId = _identityService.GetUserId();
            return JsonResult(await _sessionService.GetAllSessionsByUserIdAsync(userId, q, offset, limit));
        }

        [HttpGet("sessions/{id}")]
        public async Task<IActionResult> GetSessionById(Guid id)
        {
            return JsonResult(await _sessionService.GetSessionByIdAsync(id));
        }

        [HttpDelete("sessions")]
        public async Task<IActionResult> CloseSessions(int userId = default, bool withCurrent = false)
        {
            if (userId == default || userId < 1)
                userId = _identityService.GetUserId();
            return JsonResult(await _sessionService.CloseAllSessionsAsync(userId, withCurrent));
        }

        [HttpDelete("sessions/{id}")]
        public async Task<IActionResult> CloseSessionById(Guid sessionId)
        {
            return JsonResult(await _sessionService.CloseSessionByIdAsync(sessionId));
        }

        #endregion
    }
}