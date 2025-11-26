using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Helpers;
using VitrineApi.Interfaces;
using VitrineApi.Models;
using VitrineApi.Validators;
using VitrineApi.ViewModels;
using VitrineApi.ViewModels.Loja;

namespace VitrineApi.Controllers
{
    [AllowAnonymous]
    [Route("usuario")]
    [ApiController]
    public class UsuarioController : Controller
    {
       
        private readonly SignInManager<LojistaAuth> _signInManager;
        private readonly UserManager<LojistaAuth> _userManager;
        private readonly IConfiguration _config;
        private readonly VitrineDBContext _context;
        private readonly Cpf_CnpjValidator _cpfCnpjValidator = new Cpf_CnpjValidator();
        private readonly RepositoryBase<Lojista> _repLojista;
        private readonly DbEsgotado _dbEsgotado;

        public UsuarioController(SignInManager<LojistaAuth> signInManager, UserManager<LojistaAuth> userManager, IConfiguration config, VitrineDBContext context, Cpf_CnpjValidator cpfCnpjValidator, RepositoryBase<Lojista> repLojista, DbEsgotado dbEsgotado)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _context = context;
            _cpfCnpjValidator = cpfCnpjValidator;
            _repLojista = repLojista;
            _dbEsgotado = dbEsgotado;
        }

        [HttpGet("health")]
        public IActionResult ApiHealth()
        {
            return Ok(new { message = "A API está funcionando!" });
        }

        private string GenerateJwtToken(IdentityUser user)
        {

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM model)
        {
            if (_dbEsgotado.VerificarBancoEsgotado())
            {
                return StatusCode(500, new { message = "Banco de dados esgotado." });
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Realiza a autenticação
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Senha,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (!result.Succeeded)
                return Unauthorized(new { message = "E-mail ou senha inválidos." });

            // Recupera o usuário
            var user = await _userManager.FindByEmailAsync(model.Email);

            // Junta as consultas usando um método único para carregar as informações relacionadas
            var lojaData = await _context.Loja
                .Where(l => l.Cpf_Cnpj == user.Cpf_Cnpj)
                .Join(
                    _context.Lojista,
                    loja => loja.Cpf_Cnpj,
                    lojista => lojista.Cpf_Cnpj,
                    (loja, lojista) => new { loja, lojista }
                )
                .Join(
                    _context.CategoriaLoja,
                    lojaLojista => lojaLojista.loja.IdCategoria,
                    categoria => categoria.Id,
                    (lojaLojista, categoria) => new
                    {
                        lojaLojista.loja,
                        lojaLojista.lojista,
                        categoria
                    })
                .FirstOrDefaultAsync();

            // Verifica se a loja foi encontrada
            if (lojaData == null)
                return Ok(new { token = GenerateJwtToken(user) }); // Apenas o token e dados básicos

            var loja = lojaData.loja;
            var lojista = lojaData.lojista;
            var categoria = lojaData.categoria;

            var token = GenerateJwtToken(user);

            // Retorna os dados do usuário e loja
            return Ok(new
            {
                token,
                user = new
                {
                    id = user.Id,
                    nome = user.UserName,
                    email = user.Email,
                    cpfCnpj = user.Cpf_Cnpj,
                    telefone = lojista?.Telefone,

                    loja = loja == null ? null : new
                    {
                        id = loja.Id,
                        nomeLoja = loja.NomeLoja,
                        subdominio = loja.Subdominio,
                        idCategoria = loja.IdCategoria,
                        categoria = categoria.Titulo,
                        descricao = loja.Descricao,
                        avaliacao = loja.Avaliacao,
                        logo = loja.LogotipoUrl
                    }
                }
            });
        }

        [HttpPost("cadastrar")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Lojista.Any(l => l.Cpf_Cnpj == model.Cpf_Cnpj))
                return Ok("CPF já cadastrado.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                DateOnly dataNascimento;
                try
                {
                    var dataFormatada = model.DataNascimento.Replace('-', '/');
                    dataNascimento = DateOnly.ParseExact(dataFormatada, "dd/MM/yyyy", null);
                }
                catch (FormatException)
                {
                    return Ok(new { message = "Data de nascimento inválida. Use o formato dd-MM-yyyy ou dd/MM/yyyy." });
                }

                var lojista = new Lojista
                {
                    NomeCompleto = model.Nome,
                    Telefone = model.Telefone,
                    Cpf_Cnpj = model.Cpf_Cnpj,
                    DataNascimento = dataNascimento,
                    Email = model.Email
                };


                _context.Lojista.Add(lojista);
                await _context.SaveChangesAsync();

                var userAuth = new LojistaAuth
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Cpf_Cnpj = model.Cpf_Cnpj
                };

                var result = await _userManager.CreateAsync(userAuth, model.Senha);
                if (!result.Succeeded)
                    return BadRequest(result.Errors); // A transação vai ser descartada no `finally` se não der commit

                var loja = new Models.Loja
                {
                    NomeLoja = model.NomeLoja,
                    IdCategoria = model.IdCategoriaLoja,
                    IdTema = 1, // Tema 1 (Padrão)
                    IdLayout = 1, // Layout 1 (Padrão)
                    Cpf_Cnpj = model.Cpf_Cnpj,
                    Subdominio = model.Subdominio
                };

                _context.Loja.Add(loja);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                await _signInManager.SignInAsync(userAuth, isPersistent: false);

                return Ok(new { message = "Cadastro realizado com sucesso!" });
            }
            catch
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Erro ao realizar o cadastro.");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout bem-sucedido!" });
        }

        [HttpPost("validar-cpf-cnpj")]
        public async Task<IActionResult> ValidarCpfOuCnpj([FromBody] ValidateCpfCnpjVM request)
        {
            var cpfOuCnpj = request.Cpf_Cnpj;
            cpfOuCnpj = cpfOuCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            var cpfOucnpjExistente = await _repLojista.ListarAsync(r => r.Cpf_Cnpj == request.Cpf_Cnpj);

            if (cpfOucnpjExistente.Any())
            {
                return Ok(new { isValid = false, message = "CPF/CNPJ já cadastrado." });
            }

            if (string.IsNullOrWhiteSpace(request.Cpf_Cnpj))
            {
                return Ok(new { isValid = false, message = "CPF/CNPJ não pode ser vazio." });
            }

            // Verifica se o CPF/CNPJ é de tamanho 11 (CPF)
            if (cpfOuCnpj.Length == 11)
            {
                // Valida CPF
                if (_cpfCnpjValidator.ValidarCPF(cpfOuCnpj))
                {
                    return Ok(new { isValid = true, message = "CPF válido." });
                }
                else
                {
                    return Ok(new { isValid = false, message = "CPF inválido." });
                }
            }
            // Verifica se o CPF/CNPJ é de tamanho 14 (CNPJ)
            else if (cpfOuCnpj.Length == 14)
            {
                // Valida CNPJ
                if (_cpfCnpjValidator.ValidarCNPJ(request.Cpf_Cnpj))
                {
                    return Ok(new { isValid = true, message = "CNPJ válido." });
                }
                else
                {
                    return Ok(new { isValid = false, message = "CNPJ inválido." });
                }
            }
            else
            {
                return Ok(new { isValid = false, message = "CPF/CNPJ inválido." });
            }
        }

    }
}
