using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Interfaces;
using VitrineApi.Models;
using VitrineApi.ViewModels.Loja;

namespace VitrineApi.Controllers
{
    [AllowAnonymous]
    //[Route("loja")]
    [ApiController]
    public class LojaController : Controller
    {
        private readonly VitrineDBContext _context;
        private readonly RepositoryBase<Loja> _lojaRepo;
        private readonly ILojaService _lojaService;
        private readonly SignInManager<LojistaAuth> _signInManager;
        private readonly UserManager<LojistaAuth> _userManager;

        public LojaController(VitrineDBContext context, RepositoryBase<Loja> lojaRepo, ILojaService lojaService, SignInManager<LojistaAuth> signInManager, UserManager<LojistaAuth> userManager)
        {
            _context = context;
            _lojaRepo = lojaRepo;
            _lojaService = lojaService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("loja")]
        public async Task<IActionResult> ListarLojas()
        {
            var lojas = await _context.Loja
                .Include(l => l.IdCategoriaNavigation)
                .Include(l => l.IdLayoutNavigation)
                .Include(l => l.IdTemaNavigation)
                .ToListAsync();

            if (lojas == null || !lojas.Any())
                return Ok(new { message = "Nenhuma loja encontrada." });

            var lojasRequest = lojas.Select(loja => new LojaDto
            {
                Id = loja.Id,
                Subdominio = loja.Subdominio,
                NomeLoja = loja.NomeLoja,
                IdTema = loja.IdTema,
                IdLayout = loja.IdLayout,
                Cpf_Cnpj = loja.Cpf_Cnpj,
                Avaliacao = loja.Avaliacao ?? 0,
                Descricao = loja.Descricao,
                LogotipoUrl = loja.LogotipoUrl,
                // Navigation
                CategoriaLoja = loja.IdCategoriaNavigation?.Titulo ?? string.Empty,
                TituloTema = loja.IdTemaNavigation.Titulo,
                TituloLayout = loja.IdLayoutNavigation.Titulo
            }).ToList();

            return Ok(new { lojas = lojasRequest });
        }

        [HttpGet("loja/{id}")]
        public async Task<IActionResult> ListarLoja(int id)
        {
            var lojaSelecionada = await _context.Loja
                .Where(l => l.Id == id)
                .Include(l => l.IdCategoriaNavigation)
                .Include(l => l.IdLayoutNavigation)
                .Include(l => l.IdTemaNavigation)
                .ToListAsync();

            if (lojaSelecionada == null || !lojaSelecionada.Any())
                return Ok(new { message = "Nenhuma loja encontrada." });

            var lojaRequest = lojaSelecionada.Select(loja => new LojaDto
            {
                Id = loja.Id,
                Subdominio = loja.Subdominio,
                NomeLoja = loja.NomeLoja,
                IdTema = loja.IdTema,
                IdLayout = loja.IdLayout,
                Cpf_Cnpj = loja.Cpf_Cnpj,
                Avaliacao = loja.Avaliacao ?? 0,
                Descricao = loja.Descricao,
                LogotipoUrl = loja.LogotipoUrl,
                // Navigation
                CategoriaLoja = loja.IdCategoriaNavigation?.Titulo ?? string.Empty,
                TituloTema = loja.IdTemaNavigation.Titulo,
                TituloLayout = loja.IdLayoutNavigation.Titulo
            }).ToList();

            return Ok(new { lojaRequest });
        }

        [HttpGet("vitrine/{subdominio}")]
        public async Task<IActionResult> ListarLojaLojistaProdutos(string subdominio)
        {
            var lojaSelecionada = await _context.Loja
                .Where(l => l.Subdominio == subdominio)
                .Include(l => l.IdCategoriaNavigation)
                .Include(l => l.IdLayoutNavigation)
                .Include(l => l.IdTemaNavigation)
                .Include(l => l.Produto)
                    .ThenInclude(p => p.IdCategoriaProdutoNavigation)
                .Include(l => l.Cpf_CnpjNavigation)
                .FirstOrDefaultAsync();  // Alterado para FirstOrDefault, pois estamos esperando uma loja única

            if (lojaSelecionada == null)
                return Ok(new { message = "Nenhuma loja encontrada." });

            // Consultar as categorias de produto associadas ao IdCategoriaLoja da loja selecionada
            var categoriasDeProduto = await _context.CategoriaProduto
                .Where(c => c.IdCategoriaLoja == lojaSelecionada.IdCategoria)
                .Select(c => new CategoriaProdutoDto
                {
                    IdCategoriaProduto = c.Id,
                    TituloCategoriaProduto = c.Titulo
                })
                .ToListAsync();

            var lojaRequest = new LojaDto
            {
                Id = lojaSelecionada.Id,
                Subdominio = lojaSelecionada.Subdominio,
                NomeLoja = lojaSelecionada.NomeLoja,
                IdTema = lojaSelecionada.IdTema,
                IdLayout = lojaSelecionada.IdLayout,
                Cpf_Cnpj = lojaSelecionada.Cpf_Cnpj,
                Avaliacao = lojaSelecionada.Avaliacao ?? 0,
                Descricao = lojaSelecionada.Descricao,
                LogotipoUrl = lojaSelecionada.LogotipoUrl,
                CategoriaLoja = lojaSelecionada.IdCategoriaNavigation?.Titulo ?? string.Empty,
                TituloTema = lojaSelecionada.IdTemaNavigation.Titulo,
                TituloLayout = lojaSelecionada.IdLayoutNavigation.Titulo,
                Lojista = new LojistaDto
                {
                    NomeCompleto = lojaSelecionada.Cpf_CnpjNavigation.NomeCompleto,
                    DataNascimento = lojaSelecionada.Cpf_CnpjNavigation.DataNascimento,
                    Email = lojaSelecionada.Cpf_CnpjNavigation.Email,
                    Telefone = lojaSelecionada.Cpf_CnpjNavigation.Telefone
                },
                CategoriasProduto = categoriasDeProduto,
                Produtos = lojaSelecionada.Produto.Select(p => new ProdutoDto
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    ValorUnitario = p.ValorUnitario,
                    ValorPromocional = p.ValorPromocional,
                    ValorCusto = p.ValorCusto,
                    Estoque = p.Estoque,
                    Descricao = p.Descricao,
                    CategoriaProduto = p.IdCategoriaProdutoNavigation?.Titulo ?? string.Empty,
                    ImagemUrl = p.ImagemUrl ?? string.Empty,
                    Sku = p.Sku,
                    Ativo = p.Ativo,
                    Peso = p.Peso,
                    Altura = p.Altura,
                    Largura = p.Largura,
                    Profundidade = p.Profundidade,
                    IdCategoriaProduto = p.IdCategoriaProduto,
                    IdLoja = p.IdLoja
                }).ToList()
            };

            return Ok(new { lojaRequest });
        }



        [HttpGet("loja/listar-layouts-temas")]
        public async Task<IActionResult> ListarLayoutsTemas()
        {
            try
            {
                var layouts = await _context.Layout.ToListAsync();

                var temas = await _context.Tema.ToListAsync();

                if (layouts == null || temas == null)
                {
                    return NotFound(new { mensagem = "Nenhum layout ou tema encontrado" });
                }

                return Ok(new { layouts, temas });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensagem = "Erro ao listar layouts e temas", detalhe = ex.Message });
            }
        }

        [HttpPut("loja/alterar-layout-tema/{id}")]
        public IActionResult Put(int id, [FromBody] AlterarLayoutTemaRequest request)
        {
            var loja = _context.Loja.FirstOrDefault(l => l.Id == id);
            if (loja == null)
            {
                return NotFound(new { mensagem = "Loja não encontrada" });
            }

            bool houveAlteracao = false;

            if (request.NovoLayoutId.HasValue && request.NovoLayoutId.Value != 0)
            {
                var layout = _context.Layout.FirstOrDefault(l => l.Id == request.NovoLayoutId);
                if (layout == null)
                {
                    return BadRequest(new { mensagem = "Layout não encontrado" });
                }
                loja.IdLayout = request.NovoLayoutId.Value;
                houveAlteracao = true;
            }

            if (request.NovoTemaId.HasValue && request.NovoTemaId.Value != 0)
            {
                var tema = _context.Tema.FirstOrDefault(t => t.Id == request.NovoTemaId);
                if (tema == null)
                {
                    return BadRequest(new { mensagem = "Tema não encontrado" });
                }
                loja.IdTema = request.NovoTemaId.Value;
                houveAlteracao = true;
            }

            if (!houveAlteracao)
            {
                return Ok(new { mensagem = "Não houveram alterações." });
            }

            _context.SaveChanges();

            return Ok(new { mensagem = "Layout e/ou Tema alterados com sucesso" });
        }

        [HttpPost("loja/verificar-subdominio")]
        public async Task<IActionResult> VerificarSubdominioExistente([FromBody] VerificarSubdominioVM request)
        {
            if (string.IsNullOrWhiteSpace(request.Subdominio))
            {
                return BadRequest(new { message = "Subdomínio não pode ser vazio." });
            }

            bool existe = (await new RepositoryBase<Loja>(_context).ListarAsync(
                c => c.Subdominio.ToLower() == request.Subdominio.ToLower())).Any();

            return Ok(new { disponivel = !existe });

        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("loja/verificar-layout-tema/{idLoja}")]
        public async Task<IActionResult> VerificarLayoutTema(int idLoja)
        {
            var loja = await _context.Loja
                .Include(l => l.IdLayoutNavigation)
                .Include(l => l.IdTemaNavigation)
                .Where(l => l.Id == idLoja)
                .Select(l => new
                {
                    l.IdTema,
                    l.IdLayout,
                    TituloLayout = l.IdLayoutNavigation.Titulo,
                    TituloTema = l.IdTemaNavigation.Titulo
                })
                .FirstOrDefaultAsync();

            if (loja == null)
            {
                return BadRequest(new { message = "Loja não encontrada." });
            }

            return Ok(new
            {
                loja.IdTema,
                loja.IdLayout,
                loja.TituloLayout,
                loja.TituloTema
            });
        }
    }
}
