using Contoso.Apps.SportsLeague.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using AutoMapper;

namespace Contoso.Apps.SportsLeague.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Initialize the product database.
            Database.SetInitializer(new ProductDatabaseInitializer());

            // Automapper configuration.
            // Products:
            Mapper.CreateMap<Data.Models.Product, ViewModels.ProductViewModel>();
            Mapper.CreateMap<IList<Data.Models.Product>, IList<ViewModels.ProductViewModel>>();
            // Product list (subset of full product data):
            Mapper.CreateMap<Data.Models.Product, ViewModels.ProductListViewModel>();
            Mapper.CreateMap<IList<Data.Models.Product>, IList<ViewModels.ProductListViewModel>>();
            // Cart Items:
            Mapper.CreateMap<Data.Models.CartItem, ViewModels.CartItemViewModel>();
            Mapper.CreateMap<IList<Data.Models.CartItem>, IList<ViewModels.CartItemViewModel>>();
            // Categories:
            Mapper.CreateMap<Data.Models.Category, ViewModels.CategoryViewModel>();
            Mapper.CreateMap<IList<Data.Models.Category>, IList<ViewModels.CategoryViewModel>>();
            // Orders:
            Mapper.CreateMap<Data.Models.Order, ViewModels.OrderViewModel>();
            Mapper.CreateMap<IList<Data.Models.Order>, IList<ViewModels.OrderViewModel>>();
            Mapper.CreateMap<ViewModels.OrderViewModel, Data.Models.Order>();
        }
    }
}
