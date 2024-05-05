using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShoppingCart.DAL.Models;
using System.Reflection.Metadata.Ecma335;

namespace ShoppingCart.PL.ViewModels
{
    public class CartViewModel 
    {
        public int? Id { get; set; }

        public int ProductId { get; set; }

        [ValidateNever]
        public virtual Product? Product { get; set; } = null;

        public string ApplicationUserId { get; set; }

        [ValidateNever]
        public virtual ApplicationUser? ApplicationUser { get; set; } = null;

        public int Count { get; set; }


    }
}
