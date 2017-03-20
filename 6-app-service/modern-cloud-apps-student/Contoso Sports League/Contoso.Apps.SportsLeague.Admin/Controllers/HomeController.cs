using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Contoso.Apps.SportsLeague.Data.Logic;
using Contoso.Apps.SportsLeague.Data.Models;

namespace Contoso.Apps.SportsLeague.Admin.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            //var orderId = 2;
            //var order = new Order();
            List<Order> orders = new List<Order>();
            using (var orderActions = new OrderActions())
            {
                //order = orderActions.GetOrder(orderId);
                orders = orderActions.GetCompletedOrders();
            }

            var vm = new ViewModels.HomeViewModel
            {
                DisplayName = base.DisplayName,
                Orders = orders
            };

            if (Request.IsAuthenticated)
            {
                var user = User.Identity.Name;
            }

            return View(vm);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            var vm = new ViewModels.BaseViewModel
            {
                DisplayName = base.DisplayName
            };

            return View(vm);
        }

        public ActionResult Details(int Id)
        {
            var order = new Order();
            using (var orderActions = new OrderActions())
            {
                order = orderActions.GetOrder(Id);
            }

            var vm = new ViewModels.DetailsViewModel
            {
                DisplayName = base.DisplayName,
                Order = order
            };

            return View(vm);
        }
    }
}