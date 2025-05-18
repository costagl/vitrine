using System.ComponentModel.DataAnnotations;

namespace VitrineApi.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nome é necessário.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-mail é necessário.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é necessário.")]
        [StringLength(40, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("ConfirmPassword", ErrorMessage = "Senhas não coincidem.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmação de Senha é necessário.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public string Cpf { get; set; }
        public string DataNascimento { get; set; }
        public string Cnpj { get; set; }
        public string NomeLoja { get; set; }
        public string CategoriaVenda { get; set; }
        public string Celular { get; set; }
    }
}
