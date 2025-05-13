namespace ESports_Models
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }  // Product information
        public IEnumerable<Product> RecommendedProducts { get; set; }  // List of recommended products
    }
}
