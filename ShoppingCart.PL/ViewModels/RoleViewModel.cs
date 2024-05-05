using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCart.PL.ViewModels
{
    public class RoleViewModel
    {
        [Required]
        public string Name { get; set; }

        [ValidateNever]
        public string? Id { get; set; }
    }

    public class RoleSelectViewModel
    {

        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public bool IsSelected { get; set; }
    }

    public class UserRoleViewModel
    {
        public string ApplicationUserId { get; set; }
        public string ApplicationUserName { get; set; }
        public List<RoleSelectViewModel> Roles { get; set; }
    }
    public class UsersInRoleViewModel
    {
        public string AppUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }
        public bool IsSelected { get; set; }

    }
}

