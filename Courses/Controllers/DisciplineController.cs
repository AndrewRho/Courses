using Microsoft.AspNetCore.Mvc;

namespace Courses.Controllers;

[ApiController]
[Route("[controller]")]
public class DisciplineController
{
    [HttpPost("add")]
    public JsonResult Add(string name)
    {
        return new JsonResult(0);
    }
}