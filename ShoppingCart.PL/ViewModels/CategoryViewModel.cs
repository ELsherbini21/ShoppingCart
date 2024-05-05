using ShoppingCart.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.PL.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }

        [Range(1, 100)]
        [Display(Name = "Number of Order")]
     
        public int DisplayOrder { get; set; }

        [Display(Name = "Date Of Create ")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        public virtual Product? Product { get; set; }
    }
}
