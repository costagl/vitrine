namespace VitrineApi.DTOs
{
    public class LojaRequest
    {
        public int Id { get; set; }
        public string Subdominio { get; set; }
        public string NomeLoja { get; set; }
        public string CategoriaLoja { get; set; }
        public string Cpf_Cnpj { get; set; }
        public string LogotipoUrl { get; set; }
        public string Descricao { get; set; }
        public decimal Avaliacao { get; set; }
        public int IdTema { get; set; }
        public int IdLayout { get; set; }
        public string TituloLayout { get; set; }
        public string TituloTema { get; set; }
    }

    public class AlterarLayoutTemaRequest
    {
        public int NovoLayoutId { get; set; }
        public int NovoTemaId { get; set; }
    }

}
