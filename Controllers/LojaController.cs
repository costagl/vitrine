using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Interfaces;
using VitrineApi.Models;

namespace VitrineApi.Controllers
{
    [AllowAnonymous]
    [Route("lojas")]
    [ApiController]
    public class LojaController : Controller
    {
        private readonly VitrineDBContext _context;
        private readonly RepositoryBase<Loja> _lojaRepo;
        private readonly ILojaService _lojaService;

        public LojaController(VitrineDBContext context, RepositoryBase<Loja> lojaRepo, ILojaService lojaService)
        {
            _lojaRepo = lojaRepo;
            _context = context;
            _lojaService = lojaService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarLojas()
        {
            var lojas = await _context.Loja
                .Include(l => l.IdCategoriaNavigation)
                .Include(l => l.IdLayoutNavigation)
                .Include(l => l.IdTemaNavigation)
                .ToListAsync();

            if (lojas == null || !lojas.Any())
                return Ok(new { message = "Nenhuma loja encontrada." });

            var lojasRequest = lojas.Select(loja => new LojaRequest
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

        [HttpGet("{id}")]
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

            var lojaRequest = lojaSelecionada.Select(loja => new LojaRequest
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

        [HttpGet("listar-layout-temas")]
        public async Task<IActionResult> ListarLayoutTemas()
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


        [HttpPut("alterar-layout-tema/{id}")]
        public IActionResult Put(int id, [FromBody] AlterarLayoutTemaRequest request)
        {
            var loja = _context.Loja.FirstOrDefault(l => l.Id == id);
            if (loja == null)
            {
                return NotFound(new { mensagem = "Loja não encontrada" });
            }

            var layout = _context.Layout.FirstOrDefault(l => l.Id == request.NovoLayoutId);
            if (layout == null)
            {
                return BadRequest(new { mensagem = "Layout não encontrado" });
            }

            var tema = _context.Tema.FirstOrDefault(t => t.Id == request.NovoTemaId);
            if (tema == null)
            {
                return BadRequest(new { mensagem = "Tema não encontrado" });
            }

            loja.IdLayout = request.NovoLayoutId;
            loja.IdTema = request.NovoTemaId;

            _context.SaveChanges();

            return Ok(new { mensagem = "Layout e Tema alterados com sucesso" });
        }
    }
}
