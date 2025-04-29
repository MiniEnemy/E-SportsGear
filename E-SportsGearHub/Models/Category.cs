using System.ComponentModel.DataAnnotations;

namespace E_SportsGearHub.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int  DisplayOrder { get; set; }
        public string ImageUrl { get; set; }


    }
}
