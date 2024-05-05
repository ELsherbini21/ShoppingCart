
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShoppingCart.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.PL.ViewModels
{
    public class UsersOrdersHeaderViewModel
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        public int OrderHeaderId { get; set; }
        public string OrderHeaderName { get; set; }

        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        [ValidateNever]
        public OrderHeader OrderHeader { get; set; }

        [ValidateNever]
        public double ?OrderTotal { get; set; }

        [ValidateNever]
        public string   OrderStatus { get; set; }
    }
}
