using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Apps.SportsLeague.WorkerRole.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        public System.DateTime OrderDate { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public decimal Total { get; set; }

        public List<OrderDetailViewModel> OrderDetails { get; set; }
    }
}
