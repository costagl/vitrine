using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VitrineApi.Data;
using VitrineApi.Models;
using VitrineApi.ViewModels;

namespace VitrineApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : Controller
    {
        private readonly VitrineDBContext _context;
        private readonly SignInManager<LojistaAuth> _signInManager;
        private readonly UserManager<LojistaAuth> _userManager;

        public ProdutoController(VitrineDBContext context, SignInManager<LojistaAuth> signInManager, UserManager<LojistaAuth> userManager)
        {
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("cadastrar-produto")]
        public async Task<IActionResult> CadastrarProduto([FromBody] Produto model)
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            var userCpf = user.Cpf;

            var loja = _context.Loja.FirstOrDefault(c => c.Cpf == userCpf);

            Produto produto = new Produto()
            {
                Titulo = model.Titulo,
                IdLoja = loja.Id,
                IdCategoriaProduto = 2, // 2 = Sem Categoria
                ValorUnitario = model.ValorUnitario,
                Estoque = model.Estoque,
                Sku = model.Sku,
                Imagem = model.Imagem,
                Ativo = model.Ativo,
                Peso = model.Peso,
                Altura = model.Altura,
                Largura = model.Largura,
                Profundidade = model.Profundidade,
                Descricao = model.Descricao
            };
            return Ok(new { message = "Cadastro de produto realizado com sucesso!" });
        }

        //// teste

        //[Authorize]
        //[HttpGet("cadastrar-produto-teste-get")]
        //public async Task<IActionResult> CadastrarProdutoGet()
        //{
        //    return await CadastrarProdutoTeste();
        //}

        //[Authorize]
        //[HttpPost("cadastrar-produto-teste")]
        //public async Task<IActionResult> CadastrarProdutoTeste()
        //{
        //    var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
        //    var userCpf = user.Cpf;

        //    var loja = _context.Loja.FirstOrDefault(c => c.Cpf == userCpf);

        //    Produto produto = new Produto()
        //    {
        //        Titulo = "Camisa Polo Masculina",
        //        IdLoja = loja.Id,
        //        IdCategoriaProduto = 2,
        //        ValorUnitario = 99.90m,
        //        Estoque = 50,
        //        Sku = "CAM-POLO-001",
        //        Imagem = "https://exemplo.com/imagens/camisa-polo.jpg",
        //        Ativo = 1,
        //        Peso = 0.3m,
        //        Altura = 2.0m,
        //        Largura = 30.0m,
        //        Profundidade = 25.0m,
        //        Descricao = "Camisa polo 100% algodão, confortável e estilosa."
        //    };
        //    _context.Produto.Add(produto);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { message = "Cadastro de produto realizado com sucesso!" });
        //}
    }
}