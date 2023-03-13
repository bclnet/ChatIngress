using Microsoft.AspNetCore.Mvc;

namespace ChatIngress.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [HttpGet] public IActionResult Get() => Content($"OK: {Config.Bot}");
    }
}
