namespace ShoppingCart.DAL.Models
{
    // it's done't have implemention 
    public class OrderDetail : ModelBase
    {
        public int OrderHeaderId { get; set; }
        public virtual OrderHeader? OrderHeader { get; set; }

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public double Price { get; set; }
        public int Count { get; set; }

    }
}
