using Microsoft.AspNetCore.Mvc;

namespace Buratino.Controllers
{
    public class InvestSourceController : Controller
    {
        public IActionResult List()
        {
            return View();
        }
    }
}
