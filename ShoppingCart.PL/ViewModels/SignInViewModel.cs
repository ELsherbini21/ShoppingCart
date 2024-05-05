using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.PL.ViewModels
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }


        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        public bool RememberMe { get; set; } = false;

        public string? ReturnUrl { get; set; }
        // AuthenticationScheme is in Microsoft.AspNetCore.Authentication namespace
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
