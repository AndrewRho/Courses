using Courses.Abstractions;
using Courses.Data.Entities;
using Courses.Models;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Controllers;

[ApiController]
[Route("[controller]")]
public class TopicController
{
    [HttpPost("add")]
    public JsonResult Add(TopicEntity entity)
    {
        return new JsonResult(0);
    }
}