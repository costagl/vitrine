namespace VitrineApi.DTOs
{
    public class LojaDTO
    {
        public int Id { get; set; }
        public string Subdominio { get; set; }
        public string NomeLoja { get; set; }
        public string CategoriaLoja { get; set; }
        public int IdTema { get; set; }
        public int IdLayout { get; set; }
        public string Cpf_Cnpj { get; set; }
        public string Logotipo { get; set; }
        public string Descricao { get; set; }
        public decimal Avaliacao { get; set; }
    }
}
