    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ESports_Models
    {
        public class Product
        {
            [Key]
            public int Id { get; set; }

            [Required]
            //[Column("Product_Name")]
            public string ProductName { get; set; }

            [Required]
            public string CompanyName { get; set; }

            [Required]
            public string Description { get; set; }

            public int CategoryId { get; set; }
            [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

            [Required]
            [DisplayName("Price (NPR)")]
            [Range(1, 100000, ErrorMessage = "Price must be between NPR 1 and 100,000")]
            public double Price { get; set; }
        [ValidateNever]
            public string ImageUrl { get; set; }
          
       
        }
    }
