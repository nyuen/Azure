using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Apps.SportsLeague.Web.ViewModels
{
    public class StoreIndexViewModel
    {
        // List of products.
        public List<ProductListViewModel> Products { get; set; }
        // List of categories used to filter through the products.
        public List<CategoryViewModel> Categories { get; set; }
    }
}