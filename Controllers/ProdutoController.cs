using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Interfaces;
using VitrineApi.Models;
using VitrineApi.Mappings;
using AutoMapper;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("produto")]
public class ProdutoController : ControllerBase
{
    private readonly IRepositoryBase<Produto> _produtoRepo;
    private readonly IRepositoryBase<CategoriaProduto> _CatProdRep;
    private readonly IRepositoryBase<CategoriaLoja> _CatLojaRep;
    private readonly UserManager<LojistaAuth> _userManager;
    private readonly VitrineDBContext _context;
    private readonly IMapper _mapper;

    public ProdutoController(IRepositoryBase<Produto> produtoRepo, IRepositoryBase<CategoriaProduto> catProdRep, IRepositoryBase<CategoriaLoja> catLojaRep, UserManager<LojistaAuth> userManager, VitrineDBContext context, IMapper mapper)
    {
        _produtoRepo = produtoRepo;
        _CatProdRep = catProdRep;
        _CatLojaRep = catLojaRep;
        _userManager = userManager;
        _context = context;
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
        // model.IdCategoriaProduto = 2; // Sem Categoria

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
        //var token = Request.Headers["Authorization"];
        //Console.WriteLine("JWT recebido: " + token);

        var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

        if (user == null)
            return NotFound(new { message = "Usuário não encontrado." });

        var loja = _context.Loja.FirstOrDefault(c => c.Cpf_Cnpj == user.Cpf_Cnpj);

        if (loja == null)
            return NotFound(new { message = "Loja não encontrada." });

        var produtos = await _produtoRepo.ListarAsync(p => p.IdLoja == loja.Id);

        var produtosDto = _mapper.Map<List<ProdutoDTO>>(produtos);

        return Ok(produtosDto);
    }

    [AllowAnonymous]
    [HttpGet("categoria-produto")]
    public async Task<IActionResult> ListarCategoriasProduto()
    {
        var categoriaProduto = await _CatProdRep.ListarAsync();
        return Ok(categoriaProduto);
    }

    [AllowAnonymous]
    [HttpGet("categoria-loja")]
    public async Task<IActionResult> ListarCategoriasLoja()
    {
        var categoriaLoja = await _CatLojaRep.ListarAsync();
        return Ok(categoriaLoja);
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
        produto.IdCategoriaProduto = model.IdCategoriaProduto;

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
