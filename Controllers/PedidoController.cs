using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitrineApi.Data;
using VitrineApi.Enums;
using VitrineApi.Interfaces;
using VitrineApi.Models;
using VitrineApi.ViewModels;
using VitrineApi.Helpers;

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

        // Limpeza do CPF
        model.Cpf = new string(model.Cpf.Where(char.IsDigit).ToArray());

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // -----------------------------------------------------------
            // 1. LÓGICA DE CLIENTE
            // -----------------------------------------------------------
            Cliente cliente = null;
            try
            {
                cliente = await _context.Cliente.FirstOrDefaultAsync(c => c.Cpf == model.Cpf);

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
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao verificar ou criar cliente.", error = ex.Message });
            }

            // -----------------------------------------------------------
            // 2. LÓGICA DE ENDEREÇO
            // -----------------------------------------------------------
            EnderecoEntrega enderecoEntrega = null;
            try
            {
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
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao verificar ou criar o endereço de entrega.", error = ex.Message });
            }

            // -----------------------------------------------------------
            // 3. LÓGICA DE PEDIDOS E ITENS (COM BAIXA DE ESTOQUE)
            // -----------------------------------------------------------

            if (model.Pedidos == null || !model.Pedidos.Any())
            {
                return BadRequest(new { message = "Nenhum pedido informado." });
            }

            foreach (var pedido in model.Pedidos)
            {
                try
                {
                    if (pedido.ItensPedido == null || !pedido.ItensPedido.Any())
                    {
                        return BadRequest(new { message = $"O pedido da loja {pedido.IdLoja} não contém itens." });
                    }

                    decimal valorTotalItens = pedido.ItensPedido.Sum(item => item.Quantidade * item.PrecoUnitario);
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
                        Status = "Pendente",
                        ValorTotal = valorTotal,
                        FreteValor = pedido.FreteValor,
                    };

                    _context.Pedido.Add(pedidoEntity);
                    await _context.SaveChangesAsync(); // Gera o ID do Pedido

                    // LOOP DOS ITENS
                    foreach (var item in pedido.ItensPedido)
                    {
                        try
                        {
                            // --- NOVA LÓGICA DE ESTOQUE ---

                            // 1. Busca o produto no banco
                            var produtoDb = await _context.Produto.FindAsync(item.IdProduto);

                            if (produtoDb == null)
                            {
                                throw new Exception($"Produto ID {item.IdProduto} não encontrado.");
                            }

                            // 2. Verifica se tem estoque suficiente
                            if (produtoDb.Estoque < item.Quantidade)
                            {
                                throw new Exception($"Estoque insuficiente para o produto '{produtoDb.Titulo}'. Estoque atual: {produtoDb.Estoque}. Solicitado: {item.Quantidade}.");
                            }

                            // 3. Subtrai a quantidade
                            produtoDb.Estoque -= item.Quantidade;

                            // Marca o produto como modificado para o EF Core saber que precisa dar Update
                            _context.Produto.Update(produtoDb);

                            // ------------------------------

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
                            // Lança a exceção para que o rollback externo capture e desfaça tudo
                            throw new Exception($"Erro ao processar item {item.Titulo}: {ex.Message}");
                        }
                    }

                    // Salva os itens do pedido e as atualizações de estoque (Produto)
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Repassa o erro para o catch principal fazer o Rollback
                    throw;
                }
            }

            await transaction.CommitAsync();
            return Ok(new { success = true, message = "Pedido cadastrado com sucesso!" });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            // Logar erro ex...
            return StatusCode(500, new { message = "Ocorreu um erro fatal ao cadastrar o pedido.", error = ex.Message });
        }
    }


    [HttpGet("listar/{idLoja}")]
    public async Task<IActionResult> ListarPedidos(int idLoja)
    {
        // 1. O JOIN inicial (Mantido igual)
        var pedidosPorCliente = _context.Pedido
            .Where(p => p.IdLoja == idLoja)
            .Join(_context.Cliente,
                  pedido => pedido.CpfCliente,
                  cliente => cliente.Cpf,
                  (pedido, cliente) => new { Pedido = pedido, Cliente = cliente })
            .Join(_context.EnderecoEntrega,
                  joined => joined.Pedido.IdEnderecoEntrega,
                  endereco => endereco.Id,
                  (joined, endereco) => new { joined.Pedido, joined.Cliente, Endereco = endereco })
            .ToList();

        // 2. Agrupa por Cliente
        var pedidosVM = pedidosPorCliente
            .GroupBy(x => x.Cliente.Cpf)
            .Select(clienteGroup =>
            {
                var cliente = clienteGroup.First().Cliente;

                // 3. Agrupa por Pedido
                var pedidosDoCliente = clienteGroup
                    .GroupBy(x => x.Pedido.Id)
                    .Select(pedidoGroup =>
                    {
                        var pedido = pedidoGroup.First().Pedido;

                        // 4. ATENÇÃO: Alteração aqui para buscar o Título do Produto
                        var itensDoPedido = _context.ItensPedido
                            .Where(ip => ip.IdPedido == pedido.Id)
                            // FAZ O JOIN COM A TABELA DE PRODUTOS
                            .Join(_context.Produto,
                                  item => item.IdProduto, // Chave em ItensPedido
                                  prod => prod.Id,        // Chave em Produto (Estou assumindo que é 'Id')
                                  (item, prod) => new { Item = item, Prod = prod }) // Seleciona os dois objetos
                            .Select(joined => new ItensPedidoVM
                            {
                                IdProduto = joined.Item.IdProduto,
                                Titulo = joined.Prod.Titulo, // <--- Preenche o Título vindo da tabela Produto
                                Quantidade = joined.Item.Quantidade,
                                PrecoUnitario = joined.Item.PrecoUnitario,
                                PrecoTotal = joined.Item.PrecoTotal
                            }).ToList();

                        return new PedidoVM
                        {
                            idPedido = pedido.Id,
                            IdLoja = pedido.IdLoja,
                            IdEnderecoEntrega = pedido.IdEnderecoEntrega,
                            DataPedido = pedido.DataPedido,
                            Status = pedido.Status,
                            ValorTotal = pedido.ValorTotal,
                            FreteValor = pedido.FreteValor,
                            ItensPedido = itensDoPedido // Lista correta aninhada com Títulos
                        };
                    }).ToList();

                // 6. Constrói o ClienteEnderecoPedidoVM FINAL
                return new ClienteEnderecoPedidoVM
                {
                    Cpf = cliente.Cpf,
                    NomeCompleto = cliente.NomeCompleto,
                    Email = cliente.Email,
                    Telefone = cliente.Telefone,
                    DataCriacao = cliente.DataCriacao, // Ajustei para DataCriacao conforme seu JSON anterior
                    EnderecoEntrega = new EnderecoEntregaVM
                    {
                        EnderecoEntregaId = clienteGroup.First().Endereco.Id,
                        Logradouro = clienteGroup.First().Endereco.Logradouro,
                        Numero = clienteGroup.First().Endereco.Numero,
                        Complemento = clienteGroup.First().Endereco.Complemento,
                        Bairro = clienteGroup.First().Endereco.Bairro,
                        Cidade = clienteGroup.First().Endereco.Cidade,
                        Estado = clienteGroup.First().Endereco.Estado,
                        Cep = clienteGroup.First().Endereco.Cep
                    },
                    Pedidos = pedidosDoCliente
                };
            })
            .ToList();

        return Ok(pedidosVM);
    }

    [HttpGet("pedido/{id}")]
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
