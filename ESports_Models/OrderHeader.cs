    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace ESports_Models
    {
    public class OrderHeader
    {
        public int Id { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }

        public DateTime OrderDate { get; set; }
        public DateTime PaymentDate { get; set; }

        public double OrderTotal { get; set; }

        public string OrderStatus { get; set; } = "Pending";
        public string PaymentStatus { get; set; } = "Paid";

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }

}
