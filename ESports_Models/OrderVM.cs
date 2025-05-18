namespace ESports_Models.ViewModels
{
    public class OrderVM
    {
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<ShoppingCart> ListCart { get; set; }
        public IEnumerable<ShoppingCart> CartItems { get; set; }
        public List<ShoppingCart> CartList { get; set; }
    }
}
