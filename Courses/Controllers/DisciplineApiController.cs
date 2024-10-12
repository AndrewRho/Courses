using Courses.Abstractions;
using Courses.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Courses.Controllers;

[ApiController]
[Route("disciplineApi")]
public class DisciplineApiController : Controller
{
    private readonly ICoursesBotContextFactory _contextFactory;

    public DisciplineApiController(ICoursesBotContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    [HttpPost("add")]
    public JsonResult Add(string name)
    {
        var context = _contextFactory.GetContext();
        var existing = context.Disciplines.FirstOrDefault(x => x.Name == name);
        if (existing != null)
        {
            HttpContext.Response.StatusCode = 409;
            return new JsonResult("Ця дисціплина вже існує");
        }

        context.Disciplines.Add(new DisciplineEntity
        {
            Name = name
        });

        context.SaveChanges();

        return new JsonResult("OK");
    }

    [HttpGet("getAll")]
    public JsonResult GetAllDisciplines()
    {
        var context = _contextFactory.GetContext();
        var allDisciplies = context.Disciplines.ToArray();
        return new JsonResult(allDisciplies);
    }

    [HttpPut("update")]
    public JsonResult Update(Guid id, string name)
    {
        var context = _contextFactory.GetContext();
        var existing = context.Disciplines.FirstOrDefault(x => x.Id == id);
        if (existing == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Дисціплини з таким ID не існує");
        }

        existing.Name = name;
        context.SaveChanges();
        return new JsonResult("OK");
    }
    
    [HttpDelete("delete")]
    public JsonResult Delete(Guid id)
    {
        var context = _contextFactory.GetContext();
        var existing = context.Disciplines.Include( x => x.Topics).FirstOrDefault(x => x.Id == id);

        if (existing == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Дисціплини з таким ID не існує");
        }

        if (existing.Topics.Count > 0)
        {
            HttpContext.Response.StatusCode = 400;
            return new JsonResult("Спочатку треба видалити теми");
        }

        context.Disciplines.Remove(existing);
        context.SaveChanges();
        return new JsonResult("OK");
    }
}