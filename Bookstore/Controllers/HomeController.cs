using Bookstore.Models;
using Bookstore.Models.Domain;
using Newtonsoft.Json.Linq;
using NHibernate;
using PagedList;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Bookstore.Models.ViewController;
using Bookstore.Models.Identity;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace Bookstore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Basket()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Account()
        {
            if (Request.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Search(string TitleToSearch, int? page)
        {
            IEnumerable<Book> books;
            int pageNumber = (page ?? 1);
            var recordsOnPage = 8;

            using (ISession session = NHibernateSessions.OpenSession())
            {
                books = session.Query<Book>().ToList();
            }

            if (TitleToSearch != null)
            {
                books = from i in books where i.Title.Equals(TitleToSearch) select i;
                return View(books.ToPagedList(pageNumber, recordsOnPage));
            }



            return View(books.ToPagedList(pageNumber, recordsOnPage));
        }

        //GET
        public ActionResult Login()
        {
            return View();
        }

        //POST
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = SignInManager.PasswordSignIn(model.Login, model.Password, false, false);
                if (result == SignInStatus.Success)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            return View(model);
        }



        //GET
        public ActionResult Register()
        {
            return View();
        }

        //POST
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(RegiesterViewModel model)
        {
            var ReCaptcha = IsReCaptchValid();

            if (ModelState.IsValid && ReCaptcha)
            {
                var user = new User() { UserName = model.Login };
                var result = UserManager.Create(user, model.Password);
                if (result.Succeeded)
                {
                    SignInManager.SignIn(user, false, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }

            if (!ReCaptcha)
            {
                ModelState.AddModelError("", "Jesteś robotem?");
            }
            return View(model);
        }
        //if (ModelState.IsValid && IsReCaptchValid())
        //{

        //    using (ISession session = NHibernateSessions.OpenSession())
        //    {
        //        var user = new User() { UserName = model.Login, Password = model.Password };
        //        ViewBag.IsRobot = 0;

        //        using (var trans = session.BeginTransaction())
        //        {
        //            session.Save(user);
        //            trans.Commit();

        //        }
        //    }
        //}

        //if (!model.Equals(null) && model.Password.Equals("letmein"))
        //{
        //    return RedirectToAction("Index", "Shop");
        //}


        //return View(model);
        public UserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<UserManager>(); }
        }
        public SignInManager SignInManager
        {
            get { return HttpContext.GetOwinContext().Get<SignInManager>(); }
        }


        public bool IsReCaptchValid()
        {
            var result = false;

            var captchaResponse = Request.Form["g-recaptcha-response"];
            var secretKey = @"6Le2iVoUAAAAAPGtZXGOXNY_-eV3SD_4iHP7gsYK";
            var apiURL = @"https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}";
            var requestURI = string.Format(apiURL, secretKey, captchaResponse);
            var request = (HttpWebRequest)WebRequest.Create(requestURI);

            using (WebResponse response = request.GetResponse())
            {
                using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                {
                    JObject jResponse = JObject.Parse(stream.ReadToEnd());
                    var isSuccess = jResponse.Value<bool>("success");
                    result = (isSuccess) ? true : false;
                }
            }

            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            SignInManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}