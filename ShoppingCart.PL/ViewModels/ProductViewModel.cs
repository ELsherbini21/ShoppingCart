using Microsoft.AspNetCore.Mvc;
using ShoppingCart.DAL.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.PL.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(25)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public double Price { get; set; }


        [Display(Name = "File")]
        public IFormFile? FormFile { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; } 

        [Required]
        [Display(Name= "Category")]
        public int CategoryId { get; set; }

        public virtual Category ?Category { get; set; }
    }
}
