using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.DAL.Models
{
    public class Email
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        IList<IFormatProvider> attachements { get; set; } = null;
    }
}
