using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShoppingCart.DAL.Models;

namespace ShoppingCart.PL.ViewModels
{
    public class OrderDetailViewModel
    {
        public int Id { get; set; }

        public int OrderHeaderId { get; set; }

        [ValidateNever]
        public virtual OrderHeader? OrderHeader { get; set; }

        public int ProductId { get; set; }

        [ValidateNever]
        public virtual Product? Product { get; set; }

        public double Price { get; set; }
        public int Count { get; set; }

    }
}
