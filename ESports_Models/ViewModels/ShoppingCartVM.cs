using ESports_Models;
using System.Collections.Generic;

namespace ESports_Models.ViewModels
{
    public class ShoppingCartVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
    }
}
