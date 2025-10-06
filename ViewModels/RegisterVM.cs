using System.ComponentModel.DataAnnotations;

namespace VitrineApi.ViewModels
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Nome é necessário.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "E-mail é necessário.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é necessário.")]
        [StringLength(40, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Compare("ConfirmarSenha", ErrorMessage = "Senhas não coincidem.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Confirmação de Senha é necessário.")]
        [DataType(DataType.Password)]
        public string ConfirmarSenha { get; set; }
        public string Cpf { get; set; }
        public string DataNascimento { get; set; }
        public string Telefone { get; set; }
        public string Cnpj { get; set; }
        public string NomeLoja { get; set; }
        public string Subdominio { get; set; }
        public int IdCategoriaLoja { get; set; }
    }
}
