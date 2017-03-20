using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Apps.SportsLeague.Web.ViewModels
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string ImagePath { get; set; }

        public string ThumbnailPath { get; set; }

        public double? UnitPrice { get; set; }

        public int? CategoryID { get; set; }

        public string CategoryCategoryName { get; set; }
    }
}