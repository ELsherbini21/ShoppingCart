using ShoppingCart.DAL.Models;

namespace ShoppingCart.PL.Helpers.interfaces
{
    public interface IEmailSettings
    {
        public void SendEmail(Email email);

    }
}
