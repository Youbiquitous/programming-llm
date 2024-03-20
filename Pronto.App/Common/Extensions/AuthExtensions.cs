///////////////////////////////////////////////////////////////////
//
// Librogram: Reference application for social management of reads
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Youbiquitous.Librogram.App.Common.Security;
using Youbiquitous.Martlet.Core.Extensions;

namespace Pronto.App.Common.Extensions;

public static class AuthExtensions
{
    public static User Logged(this ClaimsPrincipal user)
    {
        var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        var display = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var role = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        var photoUrl = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Uri)?.Value;
        var provider = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationMethod)?.Value;
        //var teamId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        var controller = user.Claims.FirstOrDefault(c => c.Type == LibrogramClaimTypes.Controller)?.Value;
        var userId = user.Claims.FirstOrDefault(c => c.Type == LibrogramClaimTypes.UserId)?.Value;

        // Can't make the query here as not all users come from the local DB
        var persona = new User(email, role)
        {
            Id = userId.ToLong(),
            LastName = display,
            PhotoUrl = photoUrl,
           // AssignedRole = new RoleRepository().FindById(role),
            Email = email,
            AuthenticationProvider = provider
        };
        //if (teamId.IsNullOrWhitespace())
        //    return persona;

        //var id = teamId.ToInt();
        //persona.TeamId = id;
        return persona;
    }

    /// <summary>
    /// Creates the auth cookie for a regular user of the application
    /// </summary>
    /// <param name="context"></param>
    /// <param name="response"></param>
    /// <param name="rememberMe"></param>
    /// <returns></returns>
    public static async Task AuthenticateUser(this HttpContext context, AuthenticationResponse response, bool rememberMe)
    {
        await CreateLibrogramCookieFromAuth(context, response, rememberMe);
    }

    /// <summary>
    /// Refresh the cookie after changes to the profile
    /// </summary>
    /// <param name="context"></param>
    /// <param name="persona"></param>
    /// <returns></returns>
    public static async Task UpdateCookie(this HttpContext context, User persona)
    {
        var logged = context.User.Logged();
        logged.FirstName = persona.FirstName?.Capitalize();
        logged.LastName = persona.LastName?.Capitalize();
        logged.PhotoUrl = persona.PhotoUrl;
        await CreateLibrogramCookieInternal(context, logged, logged.AuthenticationProvider, false);
    }

    /// <summary>
    /// Convert auth info into object and create related cookie
    /// </summary>
    /// <param name="context"></param>
    /// <param name="response"></param>
    /// <param name="rememberMe"></param>
    /// <returns></returns>
    private static async Task CreateLibrogramCookieFromAuth(HttpContext context, AuthenticationResponse response, bool rememberMe)
    {
        var p = new User(response.Email, response.Role)
        {
            FirstName = response.DisplayName,
            LastName = "",
            PhotoUrl = response.PhotoUrl,
            //TeamId = response.TeamId,
            Id = response.Id.ToLong()
        };

        await CreateLibrogramCookieInternal(context, p, response.AuthenticatedBy, rememberMe);
    }

    /// <summary>
    /// Actual code to create the app cookie
    /// </summary>
    /// <param name="context"></param>
    /// <param name="persona"></param>
    /// <param name="authProvider"></param>
    /// <param name="rememberMe"></param>
    /// <returns></returns>
    private static async Task CreateLibrogramCookieInternal(HttpContext context, User persona, string authProvider, bool rememberMe)
    {
        // Create the authentication cookie
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, persona.ToString()),
            new Claim(ClaimTypes.Role, persona.RoleId),
            new Claim(ClaimTypes.Email, persona.Email),
            new Claim(ClaimTypes.Uri, persona.PhotoUrl ?? ""),
            new Claim(ClaimTypes.AuthenticationMethod, authProvider),
            //new Claim(LibrogramClaimTypes.CorporateName, $"{persona.TeamId ?? 0}"),
            new Claim(LibrogramClaimTypes.UserId, $"{persona.Id}"),
            new Claim(LibrogramClaimTypes.Controller, $"{AuthorizedRole.ControllerFor(persona.RoleId)}")
        };
        var identity = new ClaimsIdentity(claims,
            CookieAuthenticationDefaults.AuthenticationScheme);
        try
        {
            await context.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = rememberMe
                });
        }
        catch (Exception)
        {
            return;
        }
    }
}