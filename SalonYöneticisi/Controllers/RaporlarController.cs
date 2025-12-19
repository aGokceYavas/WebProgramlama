using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GymManagementApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RaporlarController : Controller
    {
        // GET: Raporlar/Index
        public IActionResult Index()
        {
            return View();
        }
    }
}