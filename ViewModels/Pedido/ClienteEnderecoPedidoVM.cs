using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VitrineApi.Enums;

public class ClienteEnderecoPedidoVM
{
    // Propriedades do Cliente
    public string Cpf { get; set; }
    public string NomeCompleto { get; set; }
    public string Email { get; set; }
    public string Telefone { get; set; }
    public DateTime DataCriacao { get; set; }

    // Propriedades de EnderecoEntrega
    public EnderecoEntregaVM EnderecoEntrega { get; set; }

    // Propriedades de Pedido
    public List<PedidoVM> Pedidos { get; set; }

    // Propriedades de ItensPedido
    public List<ItensPedidoVM>? ItensPedido { get; set; }
}

public class EnderecoEntregaVM
{
    public int EnderecoEntregaId { get; set; }
    public string Logradouro { get; set; }
    public string Numero { get; set; }
    public string Complemento { get; set; }
    public string Bairro { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Cep { get; set; }
}

public class PedidoVM
{
    public int idPedido { get; set; }
    public int IdLoja { get; set; }
    public int IdEnderecoEntrega { get; set; }
    public DateTime DataPedido { get; set; }
    public string? Status { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal FreteValor { get; set; }

    [Required(ErrorMessage = "A lista de itens não pode ser nula.")]
    [MinLength(1, ErrorMessage = "O pedido deve conter pelo menos um item.")]
    public List<ItensPedidoVM> ItensPedido { get; set; }
}

public class ItensPedidoVM
{
    public int IdProduto { get; set; }
    public string? Titulo { get; set; } 
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal PrecoTotal { get; set; }
}
