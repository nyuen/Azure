using Contoso.Apps.SportsLeague.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Contoso.Apps.SportsLeague.Admin.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public List<Order> Orders { get; set; }
    }
}