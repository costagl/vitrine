using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Helpers;
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
        private readonly DbEsgotado _dbEsgotado;

        public LojaController(VitrineDBContext context, RepositoryBase<Loja> lojaRepo, ILojaService lojaService, SignInManager<LojistaAuth> signInManager, UserManager<LojistaAuth> userManager, DbEsgotado dbEsgotado)
        {
            _context = context;
            _lojaRepo = lojaRepo;
            _lojaService = lojaService;
            _signInManager = signInManager;
            _userManager = userManager;
            _dbEsgotado = dbEsgotado;
        }

        [HttpGet("loja")]
        public async Task<IActionResult> ListarLojas()
        {
            var lojas = await _context.Loja
                .Include(l => l.IdCategoriaNavigation)
                .Include(l => l.IdLayoutNavigation)
                .Include(l => l.IdTemaNavigation)
                .Include(l => l.Cpf_CnpjNavigation)             // Inclui a navegação para Lojista
                .ThenInclude(lojista => lojista.EnderecoLojista) // Inclui o EnderecoLojista do Lojista
                .ToListAsync();

            if (lojas == null || !lojas.Any())
                return Ok(new { message = "Nenhuma loja encontrada." });

            var lojasRequest = lojas.Select(loja =>
            {
                // Pega o primeiro endereço do lojista, se existir
                var enderecoLojista = loja.Cpf_CnpjNavigation?.EnderecoLojista?.FirstOrDefault();

                return new LojaDto
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
                    ImagemBannerUrl = loja.ImagemBannerUrl,
                    // Adicionado Cidade e Estado
                    Cidade = enderecoLojista?.Cidade ?? string.Empty,
                    Estado = enderecoLojista?.Estado ?? string.Empty,
                    // Navigation
                    CategoriaLoja = loja.IdCategoriaNavigation?.Titulo ?? string.Empty,
                    TituloTema = loja.IdTemaNavigation.Titulo,
                    TituloLayout = loja.IdLayoutNavigation.Titulo
                };
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
                ImagemBannerUrl = loja.ImagemBannerUrl,
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

        [HttpPatch("loja/alterar-dados/{idLoja}")]
        public async Task<IActionResult> AlterarDadosLoja(int idLoja, [FromBody] LojaDto dto)
        {
            if (idLoja != dto.Id) return BadRequest(new { message = "ID mismatch." });

            var lojaBanco = await _context.Loja
                .Include(l => l.Cpf_CnpjNavigation)
                    .ThenInclude(lojista => lojista.EnderecoLojista)
                .FirstOrDefaultAsync(l => l.Id == idLoja);

            if (lojaBanco == null) return NotFound(new { message = "Loja não encontrada." });

            // --- ATUALIZAÇÃO DA LOJA ---
            if (!string.IsNullOrEmpty(dto.NomeLoja)) lojaBanco.NomeLoja = dto.NomeLoja;
            if (!string.IsNullOrEmpty(dto.Descricao)) lojaBanco.Descricao = dto.Descricao;
            if (!string.IsNullOrEmpty(dto.Subdominio)) lojaBanco.Subdominio = dto.Subdominio;
            if (!string.IsNullOrEmpty(dto.LogotipoUrl)) lojaBanco.LogotipoUrl = dto.LogotipoUrl;
            if (!string.IsNullOrEmpty(dto.ImagemBannerUrl)) lojaBanco.ImagemBannerUrl = dto.ImagemBannerUrl;

            // FKs (Tema e Layout)
            if (dto.IdTema.HasValue && dto.IdTema.Value > 0 && dto.IdTema.Value != lojaBanco.IdTema)
            {
                if (await _context.Tema.AnyAsync(t => t.Id == dto.IdTema.Value))
                    lojaBanco.IdTema = dto.IdTema.Value;
            }

            if (dto.IdLayout.HasValue && dto.IdLayout.Value > 0 && dto.IdLayout.Value != lojaBanco.IdLayout)
            {
                if (await _context.Layout.AnyAsync(l => l.Id == dto.IdLayout.Value))
                    lojaBanco.IdLayout = dto.IdLayout.Value;
            }

            // --- ATUALIZAÇÃO DO LOJISTA ---
            if (dto.Lojista != null && lojaBanco.Cpf_CnpjNavigation != null)
            {
                var lojistaBanco = lojaBanco.Cpf_CnpjNavigation;

                // Atualizamos apenas dados de contato e nome
                if (!string.IsNullOrEmpty(dto.Lojista.NomeCompleto)) lojistaBanco.NomeCompleto = dto.Lojista.NomeCompleto;
                if (!string.IsNullOrEmpty(dto.Lojista.Email)) lojistaBanco.Email = dto.Lojista.Email;
                if (!string.IsNullOrEmpty(dto.Lojista.Telefone)) lojistaBanco.Telefone = dto.Lojista.Telefone;

                // REMOVIDO: A lógica que atualizava DataNascimento foi apagada.
                // O campo DataNascimento permanecerá intacto no banco.

                // --- ENDEREÇO ---
                if (dto.Lojista.Endereco != null)
                {
                    var endDto = dto.Lojista.Endereco;
                    var enderecoBanco = lojistaBanco.EnderecoLojista.FirstOrDefault();

                    if (enderecoBanco == null)
                    {
                        // CRIAR NOVO
                        var novoEndereco = new EnderecoLojista
                        {
                            Cpf_CnpjLojista = lojistaBanco.Cpf_Cnpj,
                            Logradouro = endDto.Logradouro ?? "",
                            Numero = endDto.Numero ?? "S/N",
                            Bairro = endDto.Bairro ?? "",
                            Cidade = endDto.Cidade ?? "",
                            Estado = endDto.Estado ?? "",
                            Cep = endDto.Cep ?? "",
                            Complemento = endDto.Complemento
                        };
                        _context.EnderecoLojista.Add(novoEndereco);
                    }
                    else
                    {
                        // ATUALIZAR EXISTENTE
                        if (!string.IsNullOrEmpty(endDto.Logradouro)) enderecoBanco.Logradouro = endDto.Logradouro;
                        if (!string.IsNullOrEmpty(endDto.Numero)) enderecoBanco.Numero = endDto.Numero;
                        if (!string.IsNullOrEmpty(endDto.Bairro)) enderecoBanco.Bairro = endDto.Bairro;
                        if (!string.IsNullOrEmpty(endDto.Cidade)) enderecoBanco.Cidade = endDto.Cidade;
                        if (!string.IsNullOrEmpty(endDto.Estado)) enderecoBanco.Estado = endDto.Estado;
                        if (!string.IsNullOrEmpty(endDto.Cep)) enderecoBanco.Cep = endDto.Cep;
                        if (endDto.Complemento != null) enderecoBanco.Complemento = endDto.Complemento;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "Dados atualizados com sucesso." });
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Erro ao atualizar dados." });
            }
        }

        [HttpGet("loja/layout-tema/{idLoja}")]
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
