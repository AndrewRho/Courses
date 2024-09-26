using Courses.Abstractions;
using Courses.Models;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController
{
    private readonly IDisciplineRepository _disciplineRepository;
    private readonly ITopicRepository _topicRepository;

    public TopicController(
        IDisciplineRepository disciplineRepository,
        ITopicRepository topicRepository)
    {
        _disciplineRepository = disciplineRepository;
        _topicRepository = topicRepository;
    }

    [HttpPost("add")]
    public JsonResult Add(TopicModel model)
    {
        if (!_disciplineRepository.IsExists(model.DisciplineId))
        {
            return new JsonResult("Такої дисціплини не існує");
        }

        if (_topicRepository.IsExists(model))
        {
            return new JsonResult("Така тема вже існує");
        }
        
        _topicRepository.Add(model);
        return new JsonResult(0);
    }
}