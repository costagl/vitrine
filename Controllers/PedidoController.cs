using Microsoft.AspNetCore.Mvc;
using VitrineApi.Interfaces;
using VitrineApi.Models;

[ApiController]
[Route("/pedido")]
public class PedidoController : ControllerBase
{
    private readonly IRepositoryBase<Pedido> _pedidoRepository;

    public PedidoController(IRepositoryBase<Pedido> pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    [HttpPost("cadastrar")]
    public async Task<IActionResult> CadastrarPedido([FromBody] Pedido model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _pedidoRepository.IncluirAsync(model);
        return Ok(new { message = "Pedido cadastrado com sucesso!" });
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
