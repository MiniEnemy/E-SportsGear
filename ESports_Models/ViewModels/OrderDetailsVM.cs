using System.Collections.Generic;
using ESports_Models.ViewModels;
using ESports_Models;

namespace ESports_Models.ViewModels
{
    public class OrderDetailsVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
