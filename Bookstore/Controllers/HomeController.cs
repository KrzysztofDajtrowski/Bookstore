using Bookstore.Models;
using Bookstore.Models.Domain;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bookstore.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Search(string TitleToSearch)
        {
            IEnumerable<Book> books;

            using (ISession session = NHibernateSessions.OpenSession())
            {
                books = session.Query<Book>().ToList();
            }

            if (TitleToSearch != null)
            {
                books = from i in books where i.Title.Equals(TitleToSearch) select i;
                return View(books);
            }


            return View(books);
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

    }
}