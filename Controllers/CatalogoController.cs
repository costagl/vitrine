using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Interfaces;
using VitrineApi.Models;

namespace VitrineApi.Controllers
{
    [AllowAnonymous]
    [Route("lojas")]
    [ApiController]
    public class CatalogoController : Controller
    {
        private readonly VitrineDBContext _context;
        private readonly RepositoryBase<Loja> _lojaRepo;
        private readonly ILojaService _lojaService;

        public CatalogoController(VitrineDBContext context, RepositoryBase<Loja> lojaRepo, ILojaService lojaService)
        {
            _lojaRepo = lojaRepo;
            _context = context;
            _lojaService = lojaService;
        }

        [HttpGet]
        public async Task<IActionResult> ListarLojas()
        {
            var lojas = await _lojaRepo.ListarAsync();

            if (lojas == null || !lojas.Any())
                return Ok(new { message = "Nenhuma loja encontrada." });

            var lojasDTO = lojas.Select(loja => new LojaDTO
            {
                Id = loja.Id,
                Subdominio = loja.Subdominio,
                NomeLoja = loja.NomeLoja,
                IdTema = loja.IdTema,
                IdLayout = loja.IdLayout,
                Cpf = loja.Cpf,
                Cnpj = loja.Cnpj,

                CategoriaLoja = loja.IdCategoria.ToString(),
            }).ToList();

            return Ok(new { lojas = lojasDTO });
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> ListarLoja(int id)
        {
            var lojaSelecionada = await _lojaRepo.ListarAsync(l => l.Id == id);

            if (lojaSelecionada == null || !lojaSelecionada.Any())
                return Ok(new { message = "Nenhuma loja encontrada." });

            var lojasDTO = lojaSelecionada.Select(loja => new LojaDTO
            {
                Id = loja.Id,
                Subdominio = loja.Subdominio,
                NomeLoja = loja.NomeLoja,
                CategoriaLoja = loja.IdCategoriaNavigation.Titulo,
                IdTema = loja.IdTema,
                IdLayout = loja.IdLayout,
                Cpf = loja.Cpf,
                Cnpj = loja.Cnpj
            }).ToList();

            return Ok(new { lojaSelecionada });
        }

        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
