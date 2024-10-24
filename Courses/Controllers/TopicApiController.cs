using Courses.Abstractions;
using Courses.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Courses.Controllers;

[Authorize]
[ApiController]
[Route("topicApi")]
public class TopicApiController : Controller
{
    private readonly ICoursesBotContextFactory _contextFactory;

    public TopicApiController(ICoursesBotContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    [HttpPost("add")]
    public JsonResult Add(string name, int number, Guid disciplineId)
    {
        var context = _contextFactory.GetContext();
        var existingTopic = context.Topics.FirstOrDefault(x => x.Name == name && x.Discipline.Id == disciplineId);
        if (existingTopic != null)
        {
            HttpContext.Response.StatusCode = 409;
            return new JsonResult("Ця тема вже існує");
        }

        var discipline = context.Disciplines.FirstOrDefault(x => x.Id == disciplineId);
        if (discipline == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Дисціплина не знайдена");
        }

        context.Topics.Add(new TopicEntity
        {
            Discipline = discipline,
            Name = name,
            Number = number
        });

        context.SaveChanges();
        return new JsonResult("OK");
    }

    [HttpGet("getAll")]
    public JsonResult GetAll()
    {
        var context = _contextFactory.GetContext();
        var topics = context.Topics.Select(x => new {topic = x, disciplineId = x.Discipline.Id}).ToArray();
        return new JsonResult(topics);
    }
    

    [HttpPatch("update")]
    public JsonResult Update(Guid id, string? name, int? number, Guid? disciplineId)
    {
        if (name == null && number == null && disciplineId == null)
        {
            HttpContext.Response.StatusCode = 422;
            return new JsonResult("Пустий запит");
        }
        
        var context = _contextFactory.GetContext();
        var existingTopic = context.Topics.FirstOrDefault(x => x.Id == id);
        if (existingTopic == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Тему не знайдено");
        }

        if (disciplineId != null)
        {
            var discipline = context.Disciplines.FirstOrDefault(x => x.Id == disciplineId);
            if (discipline == null)
            {
                HttpContext.Response.StatusCode = 404;
                return new JsonResult("Дисціплину не знайдено");
            }

            existingTopic.Discipline = discipline;
        }

        if (!string.IsNullOrEmpty(name))
        {
            existingTopic.Name = name;
        }

        if (number != null)
        {
            existingTopic.Number = number.Value;
        }

        context.SaveChanges();
        return new JsonResult("OK");
    }

    [HttpDelete("delete")]
    public JsonResult Delete(Guid id)
    {
        var context = _contextFactory.GetContext();
        var existingTopic = context.Topics.Include( x =>x.WorkPlans).FirstOrDefault(x => x.Id == id);
        if (existingTopic == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Тему не знайдено");
        }

        if (existingTopic.WorkPlans.Count > 0)
        {
            HttpContext.Response.StatusCode = 400;
            return new JsonResult("Спочатку треба видалити робочий план");
        }

        context.Topics.Remove(existingTopic);
        context.SaveChanges();
        return new JsonResult("OK");
    }
}