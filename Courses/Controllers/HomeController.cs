using Microsoft.AspNetCore.Mvc;

namespace Courses.Controllers;


[ApiController]
[Route("home")]
public class HomeController : Controller
{

    [HttpGet("telegramLogin")]
    public ViewResult TelegramLogin()
    {
        return View();
    }
    
    
}