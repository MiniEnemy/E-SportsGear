using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESports_Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public DateTime ShippingDate { get; set; }

        [Required]
        public decimal OrderTotal { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string PostalCode { get; set; }

        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        public string PaymentStatus { get; set; }

        // Keep only this property; remove duplicate Status
        public string OrderStatus { get; set; }
        public string Status { get; set; }

        public string SessionId { get; set; }
        public string PaymentIntentId { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
