﻿using Courses.Abstractions;

namespace Courses.BotHandlers;

public class TodayBotHandler : ScheduleBotHandlerBase
{
    public TodayBotHandler(
        ITableRenderService tableRender,
        IScheduleService scheduleService) : base(tableRender, scheduleService)
    {
    }

    protected override (DateTime dateFrom, DateTime dateTo) GetTimeLimits()
    {
        var now = DateTime.Now.Date;
        var nextDay = now.AddDays(1);

        return new (now, nextDay);
    }
}