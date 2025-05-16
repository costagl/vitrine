using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Vitrine.Models
{
    public class Lojista : IdentityUser
    {
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter exatamente 11 dígitos (somente números).")]
        public string CPF { get; set; }

        [StringLength(100, ErrorMessage = "Nome completo pode ter no máximo 100 caracteres.")]
        public string NomeCompleto { get; set; }

        public DateTime DataNascimento { get; set; }

        [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ deve ter exatamente 14 dígitos (somente números).")]
        public string CNPJ { get; set; }

        [StringLength(50, ErrorMessage = "Nome da loja pode ter no máximo 50 caracteres.")]
        public string NomeLoja { get; set; }

        [StringLength(30, ErrorMessage = "Categoria da Venda pode ter no máximo 30 caracteres.")]
        public string CategoriaVenda { get; set; }

        public bool LembrarDeMim { get; set; }
    }
}