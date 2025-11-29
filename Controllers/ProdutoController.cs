using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Helpers;
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
    private readonly DbEsgotado _dbEsgotado;

    public ProdutoController(IRepositoryBase<Produto> produtoRepo, UserManager<LojistaAuth> userManager, VitrineDBContext context, ProdutoService produtoService, IMapper mapper, DbEsgotado dbEsgotado)
    {
        _produtoRepo = produtoRepo;
        _userManager = userManager;
        _context = context;
        _produtoService = produtoService;
        _mapper = mapper;
        _dbEsgotado = dbEsgotado;
    }

    [HttpGet("debug-token")]
    public IActionResult DebugToken()
    {
        var claims = User.Claims.Select(c => new { c.Type, c.Value });
        return Ok(claims);
    }

    [HttpPost("cadastrar")]
    public async Task<IActionResult> Cadastrar([FromBody] ProdutoRequest model)
    {
        if (_dbEsgotado.VerificarBancoEsgotado())
        {
            return StatusCode(500, new { message = "Banco de dados esgotado." });
        }
        var token = Request.Headers["Authorization"];
        Console.WriteLine("JWT recebido: " + token);

        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User)); ;

        if (user == null)
            return Unauthorized("Usuário não autenticado");

        var loja = _context.Loja.FirstOrDefault(c => c.Cpf_Cnpj == user.Cpf_Cnpj);

        if (loja == null)
            return BadRequest("Loja não encontrada para o usuário.");

        model.IdLoja = loja.Id;

        var produtoRequest = new Produto
        {
            Id = model.Id,                                      
            Titulo = model.Titulo,
            IdLoja = model.IdLoja,
            ValorUnitario = model.ValorUnitario,
            ValorPromocional = model.ValorPromocional,
            Estoque = model.Estoque,
            Sku = model.Sku,
            ImagemUrl = model.Imagem,
            Ativo = model.Ativo,
            Peso = model.Peso,
            Descricao = model.Descricao,
            Altura = model.Altura,
            Largura = model.Largura,
            Profundidade = model.Profundidade,
            IdCategoriaProduto = model.IdCategoriaProduto,
        };

        await _context.Produto.AddAsync(produtoRequest);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Produto cadastrado com sucesso!", produto = produtoRequest });
    }

    [AllowAnonymous]
    [HttpGet("listar/{idLoja}")]
    public async Task<IActionResult> Listar(int idLoja)
    {
        // 1. Validação básica do ID
        if (idLoja <= 0)
        {
            return BadRequest(new { message = "ID da loja inválido." });
        }

        // 2. Chamada direta ao serviço usando o ID da Loja
        // Nota: Você precisará garantir que seu _produtoService tenha um método que aceite 'int'
        var produtosDto = await _produtoService.ObterProdutosPorLojaAsync(idLoja);

        // 3. Retorno
        if (produtosDto == null || !produtosDto.Any())
        {
            // Retorna uma lista vazia junto com a mensagem para não quebrar o front-end
            return Ok(new { message = "Nenhum produto encontrado.", data = new List<object>() });
        }

        return Ok(produtosDto);
    }

    [HttpGet("{id}")]
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
    public async Task<IActionResult> Alterar(int id, [FromBody] ProdutoRequest model)
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

    [HttpDelete("remover/{id}")]
    public async Task<IActionResult> Remover(int id)
    {
        // 1. Verifica se o produto existe
        var produto = await _context.Produto.FindAsync(id); // ou seu _produtoRepo.BuscarPorIdAsync(id)
        if (produto == null)
            return NotFound(new { message = "Produto não encontrado." });

        // 2. VERIFICAÇÃO DE VENDAS (Implementação solicitada)
        // Verifica na tabela ItensPedido se existe algum registro com este IdProduto
        bool possuiVendas = await _context.ItensPedido.AnyAsync(ip => ip.IdProduto == id);

        if (possuiVendas)
        {
            // Retorna 409 Conflict avisando o front-end
            return StatusCode(409, new
            {
                message = "Não é possível excluir este produto pois existem pedidos vinculados a ele. Tente inativá-lo.",
                erro = "INTEGRIDADE_REFERENCIAL" // Opcional: código para o front tratar botão
            });
        }

        // 3. Se não tem vendas, remove normalmente
        _context.Produto.Remove(produto); // ou _produtoRepo.RemoverAsync(produto)
        await _context.SaveChangesAsync();

        return Ok(new { message = "Produto removido com sucesso." });
    }
}
