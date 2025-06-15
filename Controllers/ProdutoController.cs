using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Interfaces;
using VitrineApi.Models;

[ApiController]
[Route("produto")]
[Authorize]
public class ProdutoController : ControllerBase
{
    private readonly IRepositoryBase<Produto> _produtoRepo;
    private readonly UserManager<LojistaAuth> _userManager;
    private readonly VitrineDBContext _context;

    public ProdutoController(IRepositoryBase<Produto> produtoRepo, UserManager<LojistaAuth> userManager, VitrineDBContext context)
    {
        _produtoRepo = produtoRepo;
        _userManager = userManager;
        _context = context;
    }

    [HttpPost("cadastrar")]
    public async Task<IActionResult> Cadastrar([FromBody] Produto model)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Usuário não autenticado");

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
            return Unauthorized("Usuário não encontrado");

        var loja = _context.Loja.FirstOrDefault(c => c.Cpf == user.Cpf);

        if (loja == null)
            return BadRequest("Loja não encontrada para o usuário.");

        model.IdLoja = loja.Id;
        model.IdCategoriaProduto = 2; // Sem Categoria

        var dto = new ProdutoDTO
        {
            Id = model.Id,
            Titulo = model.Titulo,
            IdLoja = model.IdLoja,
            ValorUnitario = model.ValorUnitario,
            ValorPromocional = model.ValorPromocional,
            Estoque = model.Estoque,
            Sku = model.Sku,
            Imagem = model.Imagem,
            Ativo = model.Ativo,
            Peso = model.Peso,
            Descricao = model.Descricao,
            Altura = model.Altura,
            Largura = model.Largura,
            Profundidade = model.Profundidade,
            IdCategoriaProduto = model.IdCategoriaProduto
        };

        await _produtoRepo.IncluirAsync(model);

        return Ok(new { message = "Produto cadastrado com sucesso!", produto = dto });
    }

    [Authorize]
    [HttpGet("debug-token")]
    public IActionResult DebugToken()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }

    [HttpGet("listar")]
    public async Task<IActionResult> Listar()
    {
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

        if (user == null)
            return NotFound(new { message = "Usuário não encontrado." });

        var loja = _context.Loja.FirstOrDefault(c => c.Cpf == user.Cpf);

        if (loja == null)
            return NotFound(new { message = "Loja não encontrada." });

        var produtos = await _produtoRepo.ListarAsync(p => p.IdLoja == loja.Id);
        return Ok(produtos);
    }

    [HttpGet("listar/{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var produto = await _produtoRepo.BuscarPorIdAsync(id);
        if (produto == null)
            return NotFound("Produto não encontrado.");
        return Ok(produto);
    }

    [HttpPut("alterar/{id}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] Produto model)
    {
        var produto = await _produtoRepo.BuscarPorIdAsync(id);
        if (produto == null)
            return NotFound("Produto não encontrado.");

        produto.Titulo = model.Titulo;
        produto.ValorUnitario = model.ValorUnitario;
        produto.Estoque = model.Estoque;
        produto.Sku = model.Sku;
        produto.Imagem = model.Imagem;
        produto.Ativo = model.Ativo;
        produto.Peso = model.Peso;
        produto.Altura = model.Altura;
        produto.Largura = model.Largura;
        produto.Profundidade = model.Profundidade;
        produto.Descricao = model.Descricao;

        await _produtoRepo.AtualizarAsync(produto);

        return Ok(new { message = "Produto atualizado com sucesso." });
    }

    [HttpDelete("excluir/{id}")]
    public async Task<IActionResult> Remover(int id)
    {
        var produto = await _produtoRepo.BuscarPorIdAsync(id);
        if (produto == null)
            return NotFound("Produto não encontrado.");

        await _produtoRepo.RemoverAsync(produto);
        return Ok(new { message = "Produto removido com sucesso." });
    }
}
