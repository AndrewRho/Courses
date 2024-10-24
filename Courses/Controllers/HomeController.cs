using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Courses.Configs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Courses.Controllers;

[ApiController]
[Route("home")]
public class HomeController : Controller
{
    private readonly BotConfig _config;

    public HomeController(IOptions<BotConfig> config)
    {
        _config = config.Value;
    }


    [HttpGet]
    public ViewResult Index()
    {
        return View();
    }
    
    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("telegramCallback")]
    public async Task<IActionResult> TelegramCallback(
        string? id,
        string? first_Name, 
        string? last_Name,
        string userName,
        string? auth_Date,
        string hash)
    {
        var infoReceived = new Dictionary<string, string>();
        AddInfo("id", id, infoReceived);
        AddInfo("first_name", first_Name, infoReceived);
        AddInfo("last_name", last_Name, infoReceived);
        AddInfo("username", userName, infoReceived);
        AddInfo("auth_date", auth_Date, infoReceived);

        infoReceived = infoReceived
            .OrderBy(x => x.Key)
            .ToDictionary(x => x.Key, x => x.Value);

        var checkString = CombineString(infoReceived);
        var computedHash = GetHashHmac(checkString);

        if (computedHash.Equals(hash, StringComparison.InvariantCultureIgnoreCase))
        {
            await AuthorizeUser(userName);
        }

        return RedirectToAction("Index", "Home");
    }

    private async Task AuthorizeUser(string userName)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new(ClaimTypes.Role, "User") 
        };
        
        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true, 
            ExpiresUtc = DateTime.UtcNow.AddDays(1) 
        };
        
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);
    }


    private static void AddInfo(string key, string? value, IDictionary<string, string> meta)
    {
        if (!string.IsNullOrEmpty(value))
        {
            meta.Add(key.ToLowerInvariant(), value);
        }
    }

    private static string CombineString(IReadOnlyDictionary<string, string> meta)
    {
        var builder = new StringBuilder();

        TryAppend("auth_date");
        TryAppend("first_name");
        TryAppend("id");
        TryAppend("last_name");
        TryAppend("photo_url");
        TryAppend("username", true);

        return builder.ToString();

        void TryAppend(string key, bool isLast = false)
        {
            if (meta.ContainsKey(key))
            {
                builder.Append($"{key}={meta[key]}{(isLast ? "" : "\n")}");
            }
        }
    }
    
    private string GetHashHmac(string message)
    {
        using var hasher = SHA256.Create();
        var keyBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(_config.TelegramToken));

        var messageBytes = Encoding.UTF8.GetBytes(message);
        var hash = new HMACSHA256(keyBytes);
        var computedHash = hash.ComputeHash(messageBytes);
        return Convert.ToHexString(computedHash);
    }
}