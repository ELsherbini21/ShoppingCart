namespace ShoppingCart.DAL.Models
{
    public class Category : ModelBase
    {
        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public DateTime CreatedDateTime { get; set; } = DateTime.Now;


        public virtual ICollection<Product> Products { get; set; }
    }


}
