using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace JobSearchingSystem.Models
{
    /*public class ExternalLoginConfirmationViewModel
    {
        [Required]
        public string UserName { get; set; }
    }*/

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(150)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Họ và tên không được trống.")]
        [StringLength(50, ErrorMessage = "Họ và tên không được vượt quá 50 kí tự.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được trống.")]
        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 kí tự.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d+$", ErrorMessage = "Hãy nhập đúng định dạng số điện thoại.")]
        public string PhoneNumber { get; set; }

        public string RoleName { get; set; }
    }

    public class AccUserItem
    {
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }

    public class AccListViewModel
    {
        public IEnumerable<AccUserItem> userList { get; set; }
        public string showoption { get; set; }
    }
}
