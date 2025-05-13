using System;

namespace ESports_Models
{
    public class ProductVisit
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ApplicationUserId { get; set; }
        public int VisitCount { get; set; }
        public DateTime VisitDate { get; set; }
        public DateTime LastVisited { get; set; }

        public Product Product { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
    }
}
