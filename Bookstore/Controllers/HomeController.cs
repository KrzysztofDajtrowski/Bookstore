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
using System;
using System.Net.Mail;
using System.Net.Http;

namespace Bookstore.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Metoda zwracajaca ID zalogowanego uzytkownika
        /// </summary>
        /// <returns></returns>
        private int GetLoggedUserId()
        {
            int id;
            Int32.TryParse(User.Identity.GetUserId(), out id);
            return id;
        }
        /// <summary>
        /// Metoda czyszcząca koszyk
        /// </summary>
        /// <returns></returns>
        public ActionResult ClearOrder()
        {
            if (Request.IsAuthenticated)
            {
                var actualUserID = GetLoggedUserId();

                if (actualUserID != 0)
                {
                    using (ISession session = NHibernateSessions.OpenSession())
                    {
                        var order = session.Query<Orders>().Where(x => x.UserID.Id == actualUserID).First();
                        var ord = session.Load<Orders>(order.Id);
                        order.Active = false;
                        session.Flush();
                    }
                }

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
            
        }

        public ActionResult OrderBookInBasket()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Address", "Home", routeValues: null);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult Address(AddressViewModel model)
        {
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var actualUserID = GetLoggedUserId();
                    Orders ord;

                    if (actualUserID != 0)
                    {
                        using (ISession session = NHibernateSessions.OpenSession())
                        {
                            var order = session.Query<Orders>().Where(x => x.UserID.Id == actualUserID).First();
                            ord = session.Load<Orders>(order.Id);
                            ord.UserID = order.UserID;
                            ord.FirstName = model.FirstName;
                            ord.LastName = model.LastName;
                            ord.Street = model.Street;
                            ord.City = model.City;
                            ord.Country = model.Country;
                            session.Flush();
                        }
                        GenerateEmailWithBooks(GetBooksInCart(actualUserID), User.Identity.GetUserName(), ord);
                    }

                    ClearOrder();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return View(model);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
           



            
        }

        private bool GenerateEmailWithBooks(IEnumerable<Book> books, string emailTo, Orders order)
        {
            using (SmtpClient client = new SmtpClient())
            {
                using (MailMessage message = new MailMessage())
                {

                    string messageBody = string.Empty;

                    message.To.Add(emailTo);
                    message.IsBodyHtml = true;
                    message.Subject = "Złożone zamówienie";
                    messageBody = "<b>Złożyłeś zamówienie na następujące pozycje:</b> ";

                    foreach (var item in books)
                    {
                        messageBody = messageBody + "<br/>" + item.Title + " - " + item.Author + " - " + "CENA: " + item.Price;
                    }

                    messageBody = messageBody +  "<br/><b>Cena łączna: " + SummaryPrices(books) + "PLN</b>" +
                        "<br/><br/><b>Dane wysyłki:</b><br/>" +
                        order.LastName + " " + order.LastName + "<br/>" +
                        order.Street + "<br/>" + order.City + "<br/>" + order.Country;

                    message.Body = messageBody;

                    try
                    {
                        client.Send(message);
                        return true;
                    }
                    catch (Exception)
                    {

                        return false;
                    }
                }
            }
        }

        private bool GenerateEmailWithNewUser(string emailTo)
        {
            using (SmtpClient client = new SmtpClient())
            {
                using (MailMessage message = new MailMessage())
                {

                    string messageBody = string.Empty;

                    message.To.Add(emailTo);
                    message.IsBodyHtml = true;
                    message.Subject = "Rejestracja nowego użytkownika";
                    message.Body = "Na podanym adresie zajejestrowano nowego użytkownika" +
                        "<br/>Twoj login: " + emailTo;

                    try
                    {
                        client.Send(message);
                        return true;
                    }
                    catch (Exception)
                    {

                        return false;
                    }
                }
            }
        }

        //Get
        public ActionResult Index()
        {
            return View();
        }

        //GET
        public ActionResult Basket()
        {
            if (Request.IsAuthenticated)
            {
                var books = GetBooksInCart(GetLoggedUserId());

                if (books.Count() == 0)
                {
                    ViewBag.Empty = true;
                    return View();
                }
                else
                {
                    ViewBag.Empty = false;
                    ViewBag.BookInBasket = books.Count();
                    ViewBag.SummaryPrices = SummaryPrices(books);
                    return View(books);
                }                
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private IEnumerable<Book> GetBooksInCart(int userId)
        {
            IEnumerable<OrdersBook> bookInBasket;  
            IEnumerable<Book> books; 
            List<Book> booksInOrder = new List<Book>(); 
            using (ISession session = NHibernateSessions.OpenSession())
            {
                books = session.Query<Book>().ToList();
                bookInBasket = session.Query<OrdersBook>().Where(x => x.OrderID == IsAnyOrderExist(userId)).ToList();
            }

            books = books.AsQueryable();

            foreach (var item in bookInBasket)
            {
                var id = (long)item.BookID;
                var book = books.Where(x => x.Id == id).First();
                booksInOrder.Add( new Book { Author = book.Author, Genre = book.Genre, Price = book.Price, Title = book.Title } );
            }

            return booksInOrder.AsEnumerable();
        }

        private int SummaryPrices(IEnumerable<Book> books)
        {
            float summary = default(int);

            foreach (var item in books)
            {
                summary += item.Price;
            }

            return (int)summary;
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
                    GenerateEmailWithNewUser(user.UserName);
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
        
        //return View(model);
        public UserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<UserManager>(); }
        }
        public SignInManager SignInManager
        {
            get { return HttpContext.GetOwinContext().Get<SignInManager>(); }
        }
        /// <summary>
        /// sprawdzenie poprawnosci captch
        /// </summary>
        /// <returns></returns>
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

        [HttpGet]
        public ActionResult AddBook(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "nie przesłano ID ksiązki do dodania");
            }
            else
            {
                long bookId = id ?? default(int);
                var userId = GetLoggedUserId();


                if (IsAnyOrderExist(userId) == 0)
                {
                    CreateNewOrder(userId);
                }

                AddBookToOrder(userId, (int)bookId);
            }
            return RedirectToAction("Search", "Home");

        }

        private int IsAnyOrderExist(int userId)
        {
            using (ISession session = NHibernateSessions.OpenSession())
            {
                var isOrderActive = session.Query<Orders>().Where(x => x.UserID.Id == userId).Where(x => x.Active == true).FirstOrDefault();
                if (isOrderActive == null)
                {
                    return 0;
                }
                else
                {
                    return isOrderActive.Id;
                }

            }
        }

        private void CreateNewOrder(int userId)
        {
            using (ISession session = NHibernateSessions.OpenSession())
            {
                var currentLoggedUser = session.Query<User>().Where(x => x.Id == userId).First();

                using (var transaction = session.BeginTransaction())
                { 
                    var createOrder = new Orders() { UserID = currentLoggedUser, Active = true };
                    session.Save(createOrder);
                    transaction.Commit();
                }
            }
        }

        private void AddBookToOrder(int userId, int bookId)
        {
            using (ISession session = NHibernateSessions.OpenSession())
            {
                var activeOrder = session.Query<Orders>().Where(x => x.UserID.Id == userId).Where(x => x.Active == true).FirstOrDefault();


                if (activeOrder != null)
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var bookOrder = new OrdersBook() { BookID = bookId , OrderID = activeOrder.Id };
                        session.Save(bookOrder);
                        transaction.Commit();
                    }
                }
            }
        }

    }
}