using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Apps.SportsLeague.WorkerRole.Models
{
    public class OrderDetailViewModel
    {
        public int Quantity { get; set; }

        public double? UnitPrice { get; set; }
        public string ProductName { get; set; }

        public double Cost
        {
            get
            {
                double unitPrice = UnitPrice.HasValue ? UnitPrice.Value : 0;
                return Math.Round(unitPrice * Quantity, 2);
            }
        }
    }
}
