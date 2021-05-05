using System.ComponentModel.DataAnnotations;
using WebSite.Resources;

namespace WebSite.Models
{
    public class LoginViewModel
    {
        [Required]
        [RegularExpression(RegularExpressionConstants.Email)]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(52, MinimumLength = 6)]
        public string Email { get; set; }

        [Required]
        [RegularExpression(RegularExpressionConstants.Password)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(32, MinimumLength = 7)]
        public string Password { get; set; }
        public string ResultMessage { get; set; }
    }
}