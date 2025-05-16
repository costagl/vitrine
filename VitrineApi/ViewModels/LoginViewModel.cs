using System.ComponentModel.DataAnnotations;

namespace VitrineApi.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email é necessário.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é necessário.")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Lembrar-se de mim?")]
        public bool RememberMe { get; set; }
    }
}
