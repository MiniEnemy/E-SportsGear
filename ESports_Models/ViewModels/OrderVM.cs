using ESports_Models.ViewModels;

namespace ESports_Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
        public List<ShoppingCart> CartList { get; set; }         // For checkout/cart views
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
