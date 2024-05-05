using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.PL.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }

        public string Token { get; set; }

        [Required(ErrorMessage = "Password Is Required")]
        [MinLength(5, ErrorMessage = "Minimum Password Length is 5")]
        [MaxLength(30, ErrorMessage = "Maximum Password Length is 30")]
        [DataType(DataType.Password)]
        public string Password { get; set; }



        [Required(ErrorMessage = "Confirm Password Is Required")]
        [Compare(nameof(Password), ErrorMessage = "Confirm Password doesn't match with Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
