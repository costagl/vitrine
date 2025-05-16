using System.ComponentModel.DataAnnotations;

namespace VitrineApi.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Email é necessário.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é necessário.")]
        [StringLength(40, MinimumLength = 8, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova senha")]
        [Compare("ConfirmNewPassword", ErrorMessage = "Senhas não coincidem.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirmação de Senha é necessário.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nova senha")]
        public string ConfirmNewPassword { get; set; }
    }
}
