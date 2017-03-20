using Contoso.Apps.SportsLeague.Data.Models;
using Contoso.Apps.SportsLeague.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Contoso.Apps.SportsLeague.Data.Logic;

namespace Contoso.Apps.SportsLeague.Controllers {
    public class HomeController : Controller {
        public ActionResult Index() {
            //var orderId = 1;
            //var order = new Order();
            //using (var orderActions = new OrderActions(orderId))
            //{
            //    order = orderActions.GetOrder();
            //}

            var vm = new HomeViewModel();

            return View(vm);
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}