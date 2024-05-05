using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.PL.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
