using Courses.Abstractions;
using Courses.Configs;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Controllers;

[ApiController]
[Route(ControllerNames.Discipline)]
public class DisciplineController : Controller
{
    private readonly IDisciplineRepository _repository;

    public DisciplineController(IDisciplineRepository repository)
    {
        _repository = repository;
    }

    [HttpPost("add")]
    public JsonResult Add(string name)
    {
        if (_repository.IsExists(name))
        {
            return new JsonResult("Ця дисціплина вже існує");
        }

        _repository.Create(name);
        return new JsonResult(0);
    }

    [HttpGet(ActionNames.GetAllInfo)]
    public ViewResult GetAllInfo()
    {
        var models = _repository.GetAllWithTopics();
        return View(models);
    }


    [HttpPost("delete")]
    public JsonResult Delete(string name)
    {
        if (!_repository.IsExists(name))
        {
            return new JsonResult("Дисціплина відсутня у базі даних");
        }

        _repository.Delete(name);
        return new JsonResult(0);
    }
}