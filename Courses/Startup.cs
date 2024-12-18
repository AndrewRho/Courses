﻿using Courses.Abstractions;
using Courses.BotHandlers;
using Courses.Configs;
using Courses.Data.Entities;
using Courses.Factories;
using Courses.Implementations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Courses;

public class Startup
{
    private readonly WebApplicationBuilder _builder;
    private IHost? _application;

    public Startup()
    {
        _builder = WebApplication.CreateBuilder();
    }
    
    public void Start()
    {
        SetupBuilder();
        InitApplication();
        ApplyMigrations();
        PopulateDictionaries();
        _application.Run();
    }
    
    private void ApplyMigrations()
    {
        var contextFactory = _application?.Services.GetRequiredService<ICoursesBotContextFactory>();
        using var context = contextFactory?.GetContext();
        context?.Database.Migrate();
    }
    
    private void InitApplication()
    {
        var app = _builder.Build();

        app.UseRouting();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(e => e.MapControllers());

        app.UseSwagger();
        app.UseSwaggerUI();

        _application = app;
    }

    private void PopulateDictionaries()
    {
        if (_application == null)
        {
            return;
        }
        
        var contextFactory = _application.Services.GetRequiredService<ICoursesBotContextFactory>();
        using var context = contextFactory.GetContext();

        if (!context.TimeSlots.Any())
        {
            context.TimeSlots.AddRange(
                new TimeSlotEntity{ Id = 1, TimeFrom = "08:30", TimeTo = "09:50"},
                new TimeSlotEntity{ Id = 2, TimeFrom = "10:10", TimeTo = "11:30"},
                new TimeSlotEntity{ Id = 3, TimeFrom = "12:00", TimeTo = "13:20"},
                new TimeSlotEntity{ Id = 4, TimeFrom = "13:40", TimeTo = "15:00"},
                new TimeSlotEntity{ Id = 5, TimeFrom = "15:20", TimeTo = "16:40"},
                new TimeSlotEntity{ Id = 6, TimeFrom = "17:00", TimeTo = "18:20"});
        }

        context.SaveChanges();
    }
    
    private void SetupBuilder()
    {
        _builder.Services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.LoginPath = "/home";
                options.AccessDeniedPath = "/home";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
                options.Cookie.IsEssential = true;
            });

        _builder.Services.AddMvc();
        _builder.Services.AddControllers();

        _builder.Services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .Configure<BotConfig>(_builder.Configuration.GetSection("BotConfig"))
            .AddSingleton<ICoursesBotContextFactory, CoursesBotContextFactory>()
            .AddTransient<IBotHandlerFactory, BotHandlerFactory>()
            .AddTransient<ITableRenderService, TableRenderService>()
            .AddTransient<ITelegramRestClient, TelegramRestClient>()
            .AddTransient<IWorkPlanService, WorkPlanService>()
            .AddTransient<IScheduleService, ScheduleService>()
            .AddTransient<StartBotHandler>()
            .AddTransient<TodayBotHandler>()
            .AddTransient<WeekBotHandler>()
            .AddTransient<SemesterBotHandler>()
            .AddTransient<TextFileBotHandler>()
            .AddTransient<InfoBotHandler>()
            .AddTransient<MainMenuBotHandler>();
    }
}