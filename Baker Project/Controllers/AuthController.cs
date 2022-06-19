using Baker_Project.Models;
using Baker_Project.ViewModels.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Baker_Project.Utilies.Constant;

namespace Baker_Project.Controllers
{
    public class AuthController : Controller
    {
        private UserManager<AppUser> _userManager { get; }
        private SignInManager<AppUser> _signInManager { get; }
        private RoleManager<IdentityRole> _roleManager { get; }
        public AuthController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        public IActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInVM signInVM, string ReturnUrl)
        {
            AppUser appUser;
            if (signInVM.UsernameorEmail.Contains("@"))
            {
                appUser = await _userManager.FindByEmailAsync(signInVM.UsernameorEmail);
            }
            else
            {
                appUser = await _userManager.FindByNameAsync(signInVM.UsernameorEmail);
            }
            if (appUser == null)
            {
                ModelState.AddModelError("", "Istifadeci adiniz veya parolinuz yalnis daxil eilmisdir");
                return View(signInVM);
            }
            var result = await _signInManager.PasswordSignInAsync(appUser, signInVM.Password, signInVM.Remember, true);
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "60 deq blok olunub");
            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Istifadeci adiniz veya parolinuz yalnis daxil eilmisdir");
                return View(signInVM);
            }
            if (ReturnUrl != null) return LocalRedirect(ReturnUrl);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            AppUser appUser = new AppUser
            {
                Name = registerVM.Firstname,
                Surname = registerVM.Lastname,
                UserName = registerVM.Username,
                Email = registerVM.Email
            };
            IdentityResult result = await _userManager.CreateAsync(appUser, registerVM.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                }
            }
            await _userManager.AddToRoleAsync(appUser, UserRoles.Admin.ToString());
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Register));
        }
        public async Task CreateRoles()
        {
            foreach (var item in Enum.GetValues(typeof(UserRoles)))
            {
                if (!await _roleManager.RoleExistsAsync(item.ToString())) await _roleManager.CreateAsync(new IdentityRole(item.ToString()));
            }
        }
    }
}
