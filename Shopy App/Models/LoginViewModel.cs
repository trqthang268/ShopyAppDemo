using System.ComponentModel.DataAnnotations;

namespace Shopy_App.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="Địa chỉ Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Display(Name = "Ghi nhớ tài khoản?")]
        public bool RememberMe { get; set; }
    }
}
