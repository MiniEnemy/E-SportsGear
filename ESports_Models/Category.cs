using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ESports_Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Display order is required")]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
    }
}
