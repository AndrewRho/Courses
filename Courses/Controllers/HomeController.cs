using Microsoft.AspNetCore.Mvc;

namespace Courses.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : Controller
{
    [HttpGet("index")]
    public JsonResult Index()
    {
        return new JsonResult(0);
    }
}