using Buratino.Models.DomainService.DomainStructure;
using Buratino.Models.Entities;
using Buratino.Models.Xtensions;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace Buratino.Controllers
{
    public class  SecurityController : Controller
    {
        [AllowAnonymous]
        public IActionResult LoginPage(string url)
        {
            ViewBag.Url = url;
            ViewBag.IsLogin = true;
            return View();
        }

        [AllowAnonymous]
        public IActionResult Signin(string login, string pass, string nextUrl)
        {
            if (login == null || pass == null)
                return Redirect("/Security/LoginPage?login=true");

            login = login.ToLower().Trim();
            pass = pass.Trim();
            Account account = null;

            #region Auth DB
            account = CockieXtensions.Account.GetAll().FirstOrDefault(x => x.Email == login && !x.IsDeleted);
            if (account == null)
                return Redirect("/Security/LoginPage?login=true");
            if (account.IsBlocked)
                return Redirect("/Security/LoginPage?login=true");

            bool truePin = account.TryAutenticate(pass);


            if (!truePin)
            {
                return Redirect($"/Security/LoginPage?login=true?url={nextUrl}");
            }

            account.LastEnter = DateTime.Now;
            CockieXtensions.Account.Save(account);
            #endregion

            Authenticate(account.Id).Wait();

            if (nextUrl.NullOrEmpty() == null)
            {
                return Redirect("/Home/Index");
            }
            else
            {
                return Redirect(nextUrl.Replace("amp;", ""));
            }
        }

        private async Task Authenticate(long userId)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userId.ToString())
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public IActionResult Exit()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
            return Redirect("/Home/Index");
        }
    }
}
