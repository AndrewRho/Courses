using Courses.Abstractions;
using Courses.Configs;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Controllers;

[ApiController]
[Route(ControllerNames.Discipline)]
public class DisciplineController : Controller
{
    private readonly ICoursesBotContextFactory _contextFactory;

    public DisciplineController(ICoursesBotContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    [HttpPost("add")]
    public JsonResult Add(string name)
    {
        //if (_repository.IsExists(name))
        {
            return new JsonResult("Ця дисціплина вже існує");
        }

        //_repository.Create(name);
        return new JsonResult(0);
    }

    [HttpGet(ActionNames.GetAllInfo)]
    public ViewResult GetAllInfo()
    {
        return View();
    }


    [HttpPost("delete")]
    public JsonResult Delete(string name)
    {
        return new JsonResult(0);
    }
}