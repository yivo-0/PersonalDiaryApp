using DataAccessLibrary.DataAccess;
using DataAccessLibrary.Models;
using Diary.WebUI.Models;
using Diary.WebUI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diary.WebUI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationContext context, ILogger<AccountController> logger, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var id = _userManager.GetUserId(HttpContext.User);
            var temp = _userManager.Users.ToList().Find(x => x.Id == id);
            return View(temp);

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string userId, string code)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    RegisterViewModel model = new RegisterViewModel();
                    model.Email = user.Email;
                    model.Id = user.Id;
                    return View(model);
                }

                return ErrorView(new ErrorViewModel { Message = "Sorry, user doesn't exist" });
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, ex.Message);
                return ErrorView(new ErrorViewModel { Message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await _userManager.FindByIdAsync(model.Id);
                user.UserName = model.UserName;
                user.BirthDate = model.Year;
                var result = await _userManager.AddPasswordAsync(user, model.Password);
                result = await _userManager.UpdateAsync(user);
                await _userManager.AddToRoleAsync(user, "user");


                if (result.Succeeded)
                {
                    // set coockie
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Incorrect login and (or) password");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Delete(string id)
        {
            var user = await _context.Users.FindAsync(id);
            return PartialView("~/Views/Account/_Delete.cshtml", user);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(User _user)
        {
            var user = await _context.Users.FindAsync(_user.Id);
            user.DeleteDate = DateTime.Now.ToUniversalTime().Ticks.ToString();
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<ActionResult> Restore(string id)
        {
            var user = await _context.Users.FindAsync(id);
            user.DeleteDate = "";
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

    }
}
