
using System.ComponentModel.DataAnnotations;

namespace OutdoorGyms.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please enter your username.")]
        [Display(Name = "Username:")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [Display(Name = "Password:")]
        [UIHint("password")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}
