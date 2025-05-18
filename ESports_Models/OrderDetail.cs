using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports_Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OrderHeader")]
        public int OrderHeaderId { get; set; }
        public int OrderId { get; set; } 

        public virtual OrderHeader OrderHeader { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}