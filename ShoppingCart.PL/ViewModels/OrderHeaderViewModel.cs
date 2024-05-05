using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using ShoppingCart.DAL.Models;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.PL.ViewModels
{
    public class OrderHeaderViewModel
    {
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string ApplicationUserId { get; set; }

        [ValidateNever]
        public virtual ApplicationUser? ApplicationUser { get; set; }


        public DateTime DateOfOrder { get; set; }

        public DateTime DateOfShipping { get; set; }

        [Required]
        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set; }

        public DateTime DateOfPayment { get; set; }
        public DateTime DueDate { get; set; }

        [Phone]
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}
