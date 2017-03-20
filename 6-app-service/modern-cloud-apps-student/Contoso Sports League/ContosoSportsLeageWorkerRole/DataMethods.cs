using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contoso.Apps.SportsLeague.Data.Models;
using Contoso.Apps.SportsLeague.WorkerRole.Models;

namespace Contoso.Apps.SportsLeague.WorkerRole
{
    public static class DataMethods
    {
        public static OrderViewModel MapOrderToViewModel(Order order)
        {
            var orderVm = new OrderViewModel
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Total = order.Total,
                FirstName = order.FirstName,
                LastName = order.LastName,
                Email = order.Email,
                Address = order.Address,
                City = order.City,
                State = order.State,
                PostalCode = order.PostalCode,
                Country = order.Country,
                Phone = order.Phone
            };
            orderVm.OrderDetails = DataMethods.MapOrderDetailToViewModel(order.OrderDetails);
            return orderVm;
        }

        private static List<OrderDetailViewModel> MapOrderDetailToViewModel(List<OrderDetail> orderDetails)
        {
            List<OrderDetailViewModel> orderDetailsVm = new List<OrderDetailViewModel>();
            foreach (var orderDetail in orderDetails)
            {
                orderDetailsVm.Add(new OrderDetailViewModel
                    {
                        Quantity = orderDetail.Quantity,
                        UnitPrice = orderDetail.UnitPrice,
                        ProductName = orderDetail.Product.ProductName
                    }
                );
            }
            return orderDetailsVm;
        }
    }
}
