using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_SportsGearHub.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(30, ErrorMessage = "Category name cannot exceed 30 characters")]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Display order is required")]
        [Range(1, 100, ErrorMessage = "Display order must be between 1 and 100")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
