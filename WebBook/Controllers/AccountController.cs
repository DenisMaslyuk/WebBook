using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebBook.ViewModels; // пространство имен моделей RegisterModel и LoginModel
using WebBook.Models; // пространство имен UserContext и класса User
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace AuthApp.Controllers
{
    public class AccountController : Controller
    {
        private BookLibraryContext db;
        public AccountController(BookLibraryContext context)
        {
            db = context;
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.IsInRole("User")) return RedirectToAction("Index", "BooksUser");
            if (User.IsInRole("Admin")) return RedirectToAction("Index", "Books");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                Users user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user); // аутентификация

                    if(user.Role=="Admin") return RedirectToAction("Index", "Books");

                    return RedirectToAction("Index", "BooksUser");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Register()
        {
            if (User.IsInRole("User")) return RedirectToAction("Index", "BooksUser");
            if (User.IsInRole("Admin")) return RedirectToAction("Index", "Books");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                Users user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    user=new Users { Email = model.Email, Password = model.Password, Role="User" };
                    if (user.Role != null)
                    {
                        db.Users.Add(user);
                        await db.SaveChangesAsync();
                        await Authenticate(user);
                    }
                       // аутентификация

                    return RedirectToAction("Index", "BooksUser");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(Users user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}