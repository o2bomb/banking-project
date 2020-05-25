using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace BusinessTier.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "This is my implementation of the BankDB web app.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Users()
        {
            ViewBag.Title = "Users Page";

            return View();
        }

        public ActionResult Accounts()
        {
            ViewBag.Title = "Accounts Page";

            return View();
        }

        public ActionResult Transactions()
        {
            ViewBag.Title = "Transactions Page";

            return View();
        }

    }
}