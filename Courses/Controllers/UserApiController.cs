using Courses.Abstractions;
using Courses.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Courses.Controllers;

[Authorize]
[ApiController]
[Route("userApi")]
public class UserApiController : Controller
{
    private readonly ICoursesBotContextFactory _contextFactory;

    public UserApiController(ICoursesBotContextFactory contextFactory)
    {
        _contextFactory = contextFactory;
    }

    [HttpGet("getAll")]
    public JsonResult GetAll()
    {
        var context = _contextFactory.GetContext();
        var users = context.Users.ToArray();
        return new JsonResult(users);
    }

    [HttpPut("update")]
    public JsonResult Update(long id, string userName)
    {
        var context = _contextFactory.GetContext();
        var existingUser = context.Users.FirstOrDefault(x => x.Id == id);
        if (existingUser == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Користувача не знайдено");
        }

        existingUser.UserName = userName;
        context.SaveChanges();
        return new JsonResult("OK");
    }
    
    [HttpDelete("delete")]
    public JsonResult Delete(long id)
    {
        var context = _contextFactory.GetContext();
        var existingUser = context.Users.FirstOrDefault(x => x.Id == id);
        if (existingUser == null)
        {
            HttpContext.Response.StatusCode = 404;
            return new JsonResult("Користувача не знайдено");
        }

        var hasPlans = context.WorkPlans.Any(x => x.User.Id == id);
        if (hasPlans)
        {
            HttpContext.Response.StatusCode = 400;
            return new JsonResult("Спочатку треба видалити робочі плани");
        }

        context.Users.Remove(existingUser);
        context.SaveChanges();
        return new JsonResult("OK");
    }

}