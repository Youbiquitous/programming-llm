///////////////////////////////////////////////////////////////////
//
// PRONTO: GPT-booking proof-of-concept
// Copyright (c) Youbiquitous
//
// Author: Youbiquitous Team
//


using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Pronto.Infrastructure.Models;
using Pronto.Shared.Settings;
using Youbiquitous.Fluent.Gpt;
using Youbiquitous.Fluent.Gpt.Providers;

namespace Pronto.App;

/// <summary>
/// Bootstrap class injected in the program
/// </summary>
public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    /// <summary>
    /// Ctor laying the ground for configuration
    /// </summary>
    /// <param name="env"></param>
    public Startup(IWebHostEnvironment env)
    {
        _environment = env;

        var settingsFileName = env.IsDevelopment()
            ? "app-settings-dev.json"
            : "app-settings.json";

        var dom = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile(settingsFileName, optional: true)
            .AddEnvironmentVariables()
            .Build();
        _configuration = dom;
    }

    /// <summary>
    /// Adds core services to the list
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Clear out default loggers added by the default web host builder (program.cs)
        services.AddLogging(config =>
        {
            config.ClearProviders();
            config.AddConfiguration(_configuration.GetSection("Logging"));
            if(_environment.IsDevelopment()) 
            {
                config.AddDebug();
                config.AddConsole();
            }
        });

        // Authentication
        services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.LoginPath = new PathString("/account/login");
                options.Cookie.Name = AppSettings.AuthCookieName;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;
            });

        // Configuration
        var settings = new AppSettings();
        _configuration.Bind(settings);

        //GPT History Provider

        var historyProvider = new InMemoryHistoryProvider();
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(10);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        //DB connection string (in prod taken from env variables)
        ProntoContext.ConnectionString = settings.General.Secrets.SqlConnectionString;

        // DI
        services.AddSingleton(settings);
        services.AddSingleton(historyProvider);

        services.AddHttpContextAccessor();

        // MVC
        services.AddLocalization();
        services.AddControllersWithViews()
            .AddMvcLocalization()
            .AddRazorRuntimeCompilation();

        //Blazor
        services.AddServerSideBlazor(o => o.DetailedErrors = true);
    }
        
    /// <summary>
    /// Configures core services
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    /// <param name="settings"></param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSettings settings)
    {
        // Error handling (CHANGE THIS MANUALLY AS APPROPRIATE)
        //app.UseExceptionHandler("/app/error");
        app.UseDeveloperExceptionPage();
        app.Use(async (context, next) =>
        {
            await next();       // let it go
            if (context.Response.StatusCode == 404)
            {
                context.Request.Path = "/app/error";
                await next();
            }
        });

        app.UseCookiePolicy();
        app.UseAuthentication();

        if (env.IsProduction())
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthorization();

        app.UseSession();

        // Security response headers
        app.Use(async (context, next) =>
        {
            if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
                context.Response.Headers.Add("X-Frame-Options", "DENY");
            // SAMEORIGIN if need to use frames ourselves
            if (!context.Response.Headers.ContainsKey("X-Xss-Protection"))
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
            if (!context.Response.Headers.ContainsKey("Referrer-Policy"))
                context.Response.Headers.Add("Referrer-Policy", "no-referrer");
            await next();
        });

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Assistant}/{action=index}/{id?}");

            //Blazor
            endpoints.MapBlazorHub();
        });
    }
}