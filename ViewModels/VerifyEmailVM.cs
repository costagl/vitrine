using System.ComponentModel.DataAnnotations;

namespace VitrineApi.ViewModels
{
    public class VerifyEmailVM
    {
        [Required(ErrorMessage = "Email é necessário.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
