using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Contoso.Apps.Common.Extensions;
using Contoso.Apps.SportsLeague.Data.Models;

namespace Contoso.Apps.SportsLeague.Web.Controllers
{
    public class StoreController : Controller
    {
        private ProductContext db = new ProductContext();

        // GET: Store
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            var productsVm = Mapper.Map<List<ViewModels.ProductListViewModel>>(products);

            // Retrieve category listing:
            var categories = db.Categories.ToList();
            var categoriesVm = Mapper.Map<List<ViewModels.CategoryViewModel>>(categories);

            var vm = new ViewModels.StoreIndexViewModel
            {
                Products = productsVm,
                Categories = categoriesVm
            };

            return View(vm);
        }

        // GET: Store/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var productVm = Mapper.Map<ViewModels.ProductViewModel>(product);

            // Find related products, based on the category:
            var relatedProducts = db.Products.Where(p => p.CategoryID == product.CategoryID && p.ProductID != product.ProductID).ToList();
            var relatedProductsVm = Mapper.Map<List<ViewModels.ProductListViewModel>>(relatedProducts);

            // Retrieve category listing:
            var categories = db.Categories.ToList();
            var categoriesVm = Mapper.Map<List<ViewModels.CategoryViewModel>>(categories);

            // Retrieve "new products" as a list of three random products not equal to the displayed one:
            var newProducts = db.Products.Where(p => p.ProductID != product.ProductID).ToList().Shuffle().Take(3);

            var newProductsVm = Mapper.Map<List<ViewModels.ProductListViewModel>>(newProducts);

            var vm = new ViewModels.StoreDetailsViewModel
            {
                Product = productVm,
                RelatedProducts = relatedProductsVm,
                NewProducts = newProductsVm,
                Categories = categoriesVm
            };

            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
