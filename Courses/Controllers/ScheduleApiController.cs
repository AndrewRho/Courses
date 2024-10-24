using Courses.Abstractions;
using Courses.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Courses.Controllers;

[Authorize]
[ApiController]
[Route("scheduleApi")]
public class ScheduleApiController : Controller
{
    private readonly ICoursesBotContextFactory _contextFactory;

    public ScheduleApiController(ICoursesBotContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    [HttpPost("add")]
    public JsonResult Add(DateTime date, Guid workPlanId, int timeSlotNumber, int lectures, int practices, string progress)
    {
        var context = _contextFactory.GetContext();
        var existingWorkPlan = context.WorkPlans.FirstOrDefault(x => x.Id == workPlanId);
        if (existingWorkPlan == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Робочий план не знайдено");
        }

        if (timeSlotNumber < 1 || timeSlotNumber > 6)
        {
            HttpContext.Response.StatusCode = 400;
            return new JsonResult("Пара - з 1 до 6 включно");
        }
        
        var timeSlot = context.TimeSlots.Single(x => x.Id == timeSlotNumber);
        context.Schedules.Add(new ScheduleEntity
        {
            Date = date,
            WorkPlan = existingWorkPlan,
            TimeSlot = timeSlot,
            Lectures = lectures,
            Practices = practices,
            Progress = progress
        });

        context.SaveChanges();
        return new JsonResult("OK");
    }

    [HttpGet("getUserSchedule")]
    public JsonResult GetUserSchedule(long userId)
    {
        var context = _contextFactory.GetContext();
        var existingUser = context.Users.FirstOrDefault(x => x.Id == userId);
        if (existingUser == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Користувача не знайдено"); 
        }

        var schedules = context.Schedules
            .Include(x => x.TimeSlot)
            .Include(x => x.WorkPlan)
            .ThenInclude(x => x.Topic)
            .ThenInclude(x => x.Discipline)
            .Where(x => x.WorkPlan.User.Id == userId)
            .Select(x => new
            {
                id = x.Id,
                discipline = x.WorkPlan.Topic.Discipline.Name,
                topic = x.WorkPlan.Topic.Name,
                progess = x.Progress,
                timeSlot = x.TimeSlot.Id,
                practices = x.Practices,
                lectures = x.Lectures,
                date = x.Date
            })
            .ToArray();

        return new JsonResult(schedules);
    }
    
    [HttpPatch("update")]
    public JsonResult Update(Guid id, DateTime? date, int? timeSlotNumber, int? lectures, int? practices, string? progress)
    {
        var context = _contextFactory.GetContext();
        var existingSchedule = context.Schedules.FirstOrDefault(x => x.Id == id);
        if (existingSchedule == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Розклад не знайдено");
        }

        if (date == null && timeSlotNumber == null && lectures == null && practices == null && progress == null)
        {
            HttpContext.Response.StatusCode = 422;
            return new JsonResult("Пустий запит");
        }

        if (date != null)
        {
            existingSchedule.Date = date.Value;
        }

        if (timeSlotNumber != null)
        {
            if (timeSlotNumber.Value < 1 || timeSlotNumber.Value > 6)
            {
                HttpContext.Response.StatusCode = 400;
                return new JsonResult("Пара - з 1 до 6 включно");
            }

            existingSchedule.TimeSlot = context.TimeSlots.Single(x => x.Id == timeSlotNumber.Value);
        }

        if (lectures != null)
        {
            existingSchedule.Lectures = lectures.Value;
        }

        if (practices != null)
        {
            existingSchedule.Practices = practices.Value;
        }

        if (progress != null)
        {
            existingSchedule.Progress = progress;
        }

        context.SaveChanges();
        return new JsonResult("OK");
    }

    [HttpDelete("delete")]
    public JsonResult Delete(Guid id)
    {
        var context = _contextFactory.GetContext();
        var existingSchedule = context.Schedules.FirstOrDefault(x => x.Id == id);
        if (existingSchedule == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Розклад не знайдено"); 
        }

        context.Schedules.Remove(existingSchedule);
        context.SaveChanges();

        return new JsonResult("OK");
    }
}