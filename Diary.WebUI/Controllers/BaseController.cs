using Diary.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace Diary.WebUI.Controllers
{
    public class BaseController : Controller
    {
        public IActionResult ErrorView(ErrorViewModel model) => RedirectToAction("Error", "Base", model);
        public IActionResult Error(ErrorViewModel model) => View(model);
    }
}
