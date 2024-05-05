using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DAL.Models
{
    public class OrderHeader : ModelBase
    {
        public string Name { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }


        public DateTime DateOfOrder { get; set; }

        public DateTime DateOfShipping { get; set; }

        public double OrderTotal { get; set; }

        public string? OrderStatus { get; set; }

        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }

        public string? SessionId { get; set; }

        public string? PaymentIntentId { get; set; }

        public DateTime DateOfPayment { get; set; }
        public DateTime DueDate { get; set; }

        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
    }
}
