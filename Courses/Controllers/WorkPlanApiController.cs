using Courses.Abstractions;
using Courses.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Courses.Controllers;

[ApiController]
[Route("workPlanApi")]
public class WorkPlanApiController : Controller
{
    private readonly ICoursesBotContextFactory _contextFactory;

    public WorkPlanApiController(ICoursesBotContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }
    
    [HttpPost("add")]
    public JsonResult AddPlan(long userId, Guid topicId, int lectures, int practices)
    {
        var context = _contextFactory.GetContext();
        var existingUser = context.Users.FirstOrDefault(x => x.Id == userId);
        if (existingUser == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Користувач не знайдений");
        }

        var existingTopic = context.Topics.FirstOrDefault(x => x.Id == topicId);
        if (existingTopic == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Тема не знайдена");
        }
        
        var existingPlan = context.WorkPlans.FirstOrDefault(x => x.User.Id == userId && x.Topic.Id == topicId);
        if (existingPlan != null)
        {
            HttpContext.Response.StatusCode = 409;
            return new JsonResult("Робочий план на цю тему вже існує");
        }

        context.WorkPlans.Add(new WorkPlanEntity
        {
            User = existingUser,
            Topic = existingTopic,
            Lectures = lectures,
            Practices = practices
        });

        context.SaveChanges();
        return new JsonResult("OK");
    }
    
    [HttpGet("getUserPlans")]
    public JsonResult GetPlans(long userId)
    {
        var context = _contextFactory.GetContext();
        var existingUser = context.Users.FirstOrDefault(x => x.Id == userId);
        if (existingUser == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Користувач не знайдений");
        }

        var plans = context.WorkPlans.Where(x => x.User.Id == userId).ToArray();
        return new JsonResult(plans);
    }

    [HttpPatch("update")]
    public JsonResult Update(Guid id, int? lectures, int? practices)
    {
        if (lectures == null && practices == null)
        {
            HttpContext.Response.StatusCode = 422;
            return new JsonResult("Пустий запит.");
        }
        
        var context = _contextFactory.GetContext();
        var existingPlan = context.WorkPlans.FirstOrDefault(x => x.Id == id);
        if (existingPlan == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Робочий план не знайдено.");
        }

        if (lectures != null)
        {
            existingPlan.Lectures = lectures.Value;
        }

        if (practices != null)
        {
            existingPlan.Practices = practices.Value;
        }

        context.SaveChanges();
        return new JsonResult("OK");
    }

    [HttpDelete("delete")]
    public JsonResult Delete(Guid id)
    {
        var context = _contextFactory.GetContext();
        var existingPlan = context.WorkPlans.Include( x => x.Schedules).FirstOrDefault(x => x.Id == id);
        if (existingPlan == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Робочий план не знайдено.");
        }

        if (existingPlan.Schedules.Count > 0)
        {
            HttpContext.Response.StatusCode = 400;
            return new JsonResult("Спочкатку треба видалити розклад.");
        }

        context.WorkPlans.Remove(existingPlan);
        context.SaveChanges();
        return new JsonResult("ОК");
    }
}