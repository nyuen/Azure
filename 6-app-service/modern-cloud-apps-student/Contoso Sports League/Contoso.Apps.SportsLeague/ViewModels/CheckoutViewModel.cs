using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contoso.Apps.SportsLeague.Web.ViewModels
{
    public class CheckoutViewModel
    {
        public IList<ViewModels.CartItemViewModel> CartItems { get; set; }
        public ViewModels.OrderViewModel Order { get; set; }
        public decimal OrderTotal { get; set; }
        public int ItemsTotal { get; set; }

        public IEnumerable<SelectListItem> CreditCardTypes
        {
            get
            {
                List<string> cardTypes = new List<string>();
                cardTypes.Add("");
                cardTypes.Add("Master Card");
                cardTypes.Add("Visa");
                cardTypes.Add("Discover");
                cardTypes.Add("American Express");

                return new SelectList(cardTypes);
            }
        }
    }
}