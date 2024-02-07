using Microsoft.AspNetCore.Mvc;

namespace Buratino.Controllers
{
    public class SourceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
