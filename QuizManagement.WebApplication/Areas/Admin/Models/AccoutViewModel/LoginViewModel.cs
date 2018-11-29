using System.ComponentModel.DataAnnotations;

namespace QuizManagement.WebApplication.Areas.Admin.Models.AccoutViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Required")]
        [Display(Name = "Email")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")] public bool RememberMe { get; set; }
    }
}