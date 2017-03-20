using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Apps.SportsLeague.Web.ViewModels
{
    public class HomeViewModel
    {
        public IList<ViewModels.ProductViewModel> Products { get; set; }
    }
}