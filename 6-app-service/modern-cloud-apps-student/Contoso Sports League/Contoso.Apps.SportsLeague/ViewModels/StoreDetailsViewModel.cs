using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Apps.SportsLeague.Web.ViewModels
{
    public class StoreDetailsViewModel
    {
        // Details for the displayed product.
        public ProductViewModel Product { get; set; }
        // Products that are in the same category as the displayed product.
        public List<ProductListViewModel> RelatedProducts { get; set; }
        // Three random products that are different from the displayed one.
        public List<ProductListViewModel> NewProducts { get; set; }
        // List of categories to display as clickable links.
        public List<CategoryViewModel> Categories { get; set; }
    }
}