using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Apps.SportsLeague.Web.ViewModels
{
    public class CartViewModel
    {
        public IList<ViewModels.CartItemViewModel> CartItems { get; set; }
        public decimal OrderTotal { get; set; }
        public int ItemsTotal { get; set; }
    }
}