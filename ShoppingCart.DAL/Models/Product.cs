namespace ShoppingCart.DAL.Models
{
    public class Product : ModelBase
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public double Price { get; set; }

        public string ImageUrl { get; set; }


        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }
    }
}
