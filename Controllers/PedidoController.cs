using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitrineApi.Data;
using VitrineApi.Enums;
using VitrineApi.Interfaces;
using VitrineApi.Models;
using VitrineApi.ViewModels;
using VitrineApi.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

[ApiController]
[Route("/pedido")]
public class PedidoController : ControllerBase
{
    private readonly VitrineDBContext _context;
    private readonly IRepositoryBase<Pedido> _pedidoRepository;
    private readonly DbEsgotado _dbEsgotado;

    public PedidoController(VitrineDBContext context, IRepositoryBase<Pedido> pedidoRepository, DbEsgotado dbEsgotado)
    {
        _context = context;
        _pedidoRepository = pedidoRepository;
        _dbEsgotado = dbEsgotado;
    }


    //[HttpGet("db-esgotado")]
    //public IActionResult TesteDbEsgotado()
    //{
    //    if (_dbEsgotado.VerificarBancoEsgotado())
    //    {
    //        return Ok(new { message = "Banco de dados esgotado." });
    //    }
    //    else
    //    {
    //        return Ok(new { message = "Ainda há armazenamento no banco de dados." });
    //    }
    //}

        [HttpPost("cadastrar")]
    public async Task<IActionResult> CadastrarPedido([FromBody] ClienteEnderecoPedidoVM model)
    {

        if (_dbEsgotado.VerificarBancoEsgotado())
        {
            return StatusCode(500, new { message = "Banco de dados esgotado." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Removido caracteres não numéricos do CPF
        model.Cpf = new string(model.Cpf.Where(char.IsDigit).ToArray());

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            Cliente cliente = null;

            try
            {
                cliente = await _context.Cliente
                    .FirstOrDefaultAsync(c => c.Cpf == model.Cpf);

                if (cliente == null)
                {
                    cliente = new Cliente
                    {
                        Cpf = model.Cpf,
                        NomeCompleto = model.NomeCompleto,
                        Email = model.Email,
                        Telefone = model.Telefone,
                        DataCriacao = DateTime.UtcNow
                    };

                    _context.Cliente.Add(cliente);
                    Console.WriteLine("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao verificar ou criar cliente.", error = ex.Message });
            }

            EnderecoEntrega enderecoEntrega = null;
            try
            {
                // Aqui acessamos diretamente o objeto `EnderecoEntrega` (não mais um array)
                var endereco = model.EnderecoEntrega;

                if (endereco != null)
                {
                    enderecoEntrega = await _context.EnderecoEntrega
                        .FirstOrDefaultAsync(e => e.CpfCliente == model.Cpf &&
                                                  e.Numero == endereco.Numero &&
                                                  e.Cep == endereco.Cep);

                    if (enderecoEntrega == null)
                    {
                        enderecoEntrega = new EnderecoEntrega
                        {
                            CpfCliente = cliente.Cpf,
                            Logradouro = endereco.Logradouro,
                            Numero = endereco.Numero,
                            Complemento = endereco.Complemento,
                            Bairro = endereco.Bairro,
                            Cidade = endereco.Cidade,
                            Estado = endereco.Estado,
                            Cep = endereco.Cep
                        };

                        _context.EnderecoEntrega.Add(enderecoEntrega);
                        await _context.SaveChangesAsync(); // Salvando o endereço
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao verificar ou criar o endereço de entrega.", error = ex.Message });
            }

            if (model.Pedidos[0].IdLoja == 0)
            {
                return StatusCode(500, new { message = "Erro ao verificar ou criar pedido" });
            }

            foreach (var pedido in model.Pedidos)
            {
                try
                {
                    decimal valorTotalItens = model.ItensPedido.Sum(item => item.Quantidade * item.PrecoUnitario);
                    decimal valorTotal = valorTotalItens + pedido.FreteValor;

                    int idEnderecoEntrega = enderecoEntrega?.Id ?? 0;

                    var brasilTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
                    var horaBrasilia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, brasilTimeZone);

                    var pedidoEntity = new Pedido
                    {
                        CpfCliente = cliente.Cpf,
                        IdLoja = pedido.IdLoja,
                        IdEnderecoEntrega = idEnderecoEntrega,
                        DataPedido = horaBrasilia,
                        Status = StatusPedido.Pendente,
                        ValorTotal = valorTotal,
                        FreteValor = pedido.FreteValor,
                    };

                    _context.Pedido.Add(pedidoEntity);
                    await _context.SaveChangesAsync();

                    foreach (var item in model.ItensPedido)
                    {
                        try
                        {
                            var itemPedido = new ItensPedido
                            {
                                IdPedido = pedidoEntity.Id,
                                IdProduto = item.IdProduto,
                                Quantidade = item.Quantidade,
                                PrecoUnitario = item.PrecoUnitario,
                                PrecoTotal = item.PrecoTotal
                            };

                            _context.ItensPedido.Add(itemPedido);
                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, new { message = "Erro ao adicionar itens ao pedido.", error = ex.Message });
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Erro ao criar pedido.", error = ex.Message });
                }
            }

            await transaction.CommitAsync();

            return Ok(new { success = true, message = "Pedido cadastrado com sucesso!" });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            return StatusCode(500, new { message = "Ocorreu um erro ao cadastrar o pedido.", error = ex.Message });
        }
    }


    [HttpGet("listar")]
    public async Task<IActionResult> ListarPedidos()
    {
        var pedidos = await _pedidoRepository.ListarAsync();
        return Ok(pedidos);
    }

    [HttpGet("listar/{id}")]
    public async Task<IActionResult> ObterPedidoPorId(int id)
    {
        var pedido = await _pedidoRepository.BuscarPorIdAsync(id);
        if (pedido == null)
            return NotFound(new { message = "Pedido não encontrado." });

        return Ok(pedido);
    }

    [HttpPut("atualizar/{id}")]
    public async Task<IActionResult> AtualizarPedido(int id, [FromBody] Pedido model)
    {
        var pedidoExistente = await _pedidoRepository.BuscarPorIdAsync(id);
        if (pedidoExistente == null)
            return NotFound(new { message = "Pedido não encontrado." });

        model.Id = id;
        await _pedidoRepository.AtualizarAsync(model);
        return Ok(new { message = "Pedido atualizado com sucesso!" });
    }

    [HttpDelete("remover/{id}")]
    public async Task<IActionResult> RemoverPedido(int id)
    {
        var pedido = await _pedidoRepository.BuscarPorIdAsync(id);
        if (pedido == null)
            return NotFound(new { message = "Pedido não encontrado." });

        await _pedidoRepository.RemoverAsync(pedido);
        return Ok(new { message = "Pedido removido com sucesso!" });
    }
}
