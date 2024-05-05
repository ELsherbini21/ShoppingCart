namespace ShoppingCart.DAL.Models
{
    public class Cart : ModelBase
    {

        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }

        public int Count { get; set; }
    }



}
