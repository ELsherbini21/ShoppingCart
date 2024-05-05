using ShoppingCart.DAL.Models;

namespace ShoppingCart.PL.ViewModels
{
    public class OrderViewModel
    {
        public int OrderHeaderId { get; set; }
        public OrderHeader OrderHeader { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        public double OrderTotal { get; set; }
    }
}
