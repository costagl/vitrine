public class PedidoRecenteVM
{
    public int IdPedido { get; set; }
    public string Status { get; set; }
    // O campo com a string formatada "Há X horas/dias"
    public string TempoDecorrido { get; set; }
}