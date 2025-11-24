using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Interfaces;
using VitrineApi.Mappings;
using VitrineApi.Models;
using VitrineApi.Services;
using VitrineApi.ViewModels.Loja;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("produto")]
public class ProdutoController : ControllerBase
{
    private readonly IRepositoryBase<Produto> _produtoRepo;
    private readonly UserManager<LojistaAuth> _userManager;
    private readonly VitrineDBContext _context;
    private readonly ProdutoService _produtoService;
    private readonly IMapper _mapper;

    public ProdutoController(IRepositoryBase<Produto> produtoRepo, UserManager<LojistaAuth> userManager, VitrineDBContext context, ProdutoService produtoService, IMapper mapper)
    {
        _produtoRepo = produtoRepo;
        _userManager = userManager;
        _context = context;
        _produtoService = produtoService;
        _mapper = mapper;
    }

    [HttpPost("cadastrar")]
    public async Task<IActionResult> Cadastrar([FromBody] Produto model)
    {
        var token = Request.Headers["Authorization"];
        Console.WriteLine("JWT recebido: " + token);

        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User)); ;

        if (user == null)
            return Unauthorized("Usuário não autenticado");

        var loja = _context.Loja.FirstOrDefault(c => c.Cpf_Cnpj == user.Cpf_Cnpj);

        if (loja == null)
            return BadRequest("Loja não encontrada para o usuário.");

        model.IdLoja = loja.Id;

        var dto = new ProdutoDTO
        {
            Id = model.Id,
            Titulo = model.Titulo,
            IdLoja = model.IdLoja,
            ValorUnitario = model.ValorUnitario,
            ValorPromocional = model.ValorPromocional,
            Estoque = model.Estoque,
            Sku = model.Sku,
            Imagem = model.ImagemUrl,
            Ativo = model.Ativo,
            Peso = model.Peso,
            Descricao = model.Descricao,
            Altura = model.Altura,
            Largura = model.Largura,
            Profundidade = model.Profundidade,
            IdCategoriaProduto = model.IdCategoriaProduto,
        };

        await _context.Produto.AddAsync(model);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Produto cadastrado com sucesso!", produto = dto });
    }

    [HttpGet("debug-token")]
    public IActionResult DebugToken()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("listar")]
    public async Task<IActionResult> Listar()
    {
        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

        if (user == null)
            return NotFound(new { message = "Usuário não encontrado." });

        var produtosDto = await _produtoService.ObterProdutosPorLojaAsync(user.Cpf_Cnpj);

        if (produtosDto == null || !produtosDto.Any())
            return Ok(new { message = "Nenhum produto encontrado.", produtosDto });

        return Ok(produtosDto);
    }

    [HttpGet("listar/{id}")]
    public async Task<IActionResult> Buscar(int id)
    {
        var produtoData = await _produtoService.ObterDetalhesProdutoAsync(id);

        if (produtoData == null)
            return NotFound( new { message = "Produto não encontrado." } );

        return Ok(new
        {
            produto = produtoData
        });
    }

    [HttpPut("alterar/{id}")]
    public async Task<IActionResult> Alterar(int id, [FromBody] AlterarProdutoVM model)
    {
        model.Id = id;
        if (id != model.Id)
        {
            return BadRequest("O ID do produto não corresponde ao ID fornecido na URL.");
        }

        var produto = await _context.Produto.FindAsync(id);

        if (produto == null)
        {
            return NotFound("Produto não encontrado.");
        }

        produto.Titulo = model.Titulo;
        produto.ValorUnitario = model.ValorUnitario;
        produto.ValorPromocional = model.ValorPromocional;
        produto.Estoque = model.Estoque;
        produto.Sku = model.Sku;
        produto.ImagemUrl = model.Imagem;
        produto.Ativo = model.Ativo;
        produto.Peso = model.Peso;
        produto.Descricao = model.Descricao;
        produto.Altura = model.Altura;
        produto.Largura = model.Largura;
        produto.Profundidade = model.Profundidade;
        produto.IdCategoriaProduto = model.IdCategoriaProduto;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Produto.Any(e => e.Id == id))
            {
                return NotFound("Produto não encontrado para atualização.");
            }
            else
            {
                throw;
            }
        }

        return Ok(produto);
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
