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
using VitrineApi.Interfaces;
using VitrineApi.Models;
using VitrineApi.ViewModels;
using VitrineApi.ViewModels.Loja;
using VitrineApi.Validators;

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

        public UsuarioController(SignInManager<LojistaAuth> signInManager, UserManager<LojistaAuth> userManager, IConfiguration config, VitrineDBContext context, Cpf_CnpjValidator cpfCnpjValidator, RepositoryBase<Lojista> repLojista)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _context = context;
            _cpfCnpjValidator = cpfCnpjValidator;
            _repLojista = repLojista;
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
                        logo = loja.Logotipo
                    }
                }
            });
        }


        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] LoginVM model)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    // Realiza a autenticação
        //    var result = await _signInManager.PasswordSignInAsync(
        //        model.Email,
        //        model.Senha,
        //        model.RememberMe,
        //        lockoutOnFailure: false
        //    );

        //    if (result.Succeeded)
        //    {
        //        // Recupera o usuário
        //        var user = await _userManager.FindByEmailAsync(model.Email);

        //        var loja = await _context.Loja.FirstOrDefaultAsync(l => l.Cpf_Cnpj == user.Cpf_Cnpj);
        //        var lojista = await _context.Lojista.FirstOrDefaultAsync(l => l.Cpf_Cnpj == user.Cpf_Cnpj);
        //        var categoria = await _context.CategoriaLoja.FirstOrDefaultAsync(c => c.Id == loja.IdCategoria);

        //        var token = GenerateJwtToken(user);

        //        // Retorna os dados do usuário e loja
        //        return Ok(new
        //        {
        //            token,
        //            user = new
        //            {
        //                id = user.Id,
        //                nome = user.UserName,
        //                email = user.Email,
        //                cpfCnpj = user.Cpf_Cnpj,
        //                telefone = lojista?.Telefone, // Usando operador de nulidade opcional

        //                loja = loja == null ? null : new
        //                {
        //                    id = loja.Id,
        //                    nomeLoja = loja.NomeLoja,
        //                    subdominio = loja.Subdominio,
        //                    idCategoria = loja.IdCategoria,
        //                    categoria = categoria.Titulo, // Usando operador de nulidade opcional
        //                    descricao = loja.Descricao,
        //                    avaliacao = loja.Avaliacao,
        //                    logo = loja.Logotipo
        //                }
        //            }
        //        });
        //    }

        //    return Unauthorized(new { message = "E-mail ou senha inválidos." });
        //}


        [HttpPost("cadastrar")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Lojista.Any(l => l.Cpf_Cnpj == model.Cpf_Cnpj))
                return BadRequest("CPF já cadastrado.");

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
                    return BadRequest(new { message = "Data de nascimento inválida. Use o formato dd-MM-yyyy ou dd/MM/yyyy." });
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

                var loja = new Loja
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

        // TODO: Métodos HTTP da LOJA

        [HttpPost("verificar-subdominio")]
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
        [HttpGet("verificar-layout-tema")]
        public async Task<IActionResult> VerificarLayoutTema()
        {
            var token = Request.Headers["Authorization"];
            Console.WriteLine("JWT recebido: " + token);

            // Encontrar o usuário pelo ID (obtido do contexto de autenticação)
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            if (user == null)
            {
                return BadRequest(new { message = "Usuário não autenticado." });
            }

            string userCpf = user.Cpf_Cnpj;

            // Buscar a loja associada ao CPF do usuário
            var loja = await _context.Loja
                .Where(l => l.Cpf_Cnpj == user.Cpf_Cnpj)
                .Select(l => new { l.IdTema, l.IdLayout })
                .FirstOrDefaultAsync();

            if (loja == null)
            {
                return BadRequest(new { message = "Loja não encontrada para o usuário." });
            }

            return Ok(new { loja.IdTema, loja.IdLayout });
        }

        [HttpPost("validar-cpf-cnpj")]
        public async Task<IActionResult> ValidarCpfOuCnpj([FromBody] ValidateCpfCnpjVM request)
        {
            var cpfOuCnpj = request.Cpf_Cnpj;
            cpfOuCnpj = cpfOuCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

            var cpfOucnpjExistente = await _repLojista.ListarAsync(r => r.Cpf_Cnpj == request.Cpf_Cnpj);

            if (cpfOucnpjExistente.Any())
            {
                return BadRequest(new { isValid = false, message = "CPF/CNPJ já cadastrado." });
            }

            if (string.IsNullOrWhiteSpace(request.Cpf_Cnpj))
            {
                return BadRequest(new { isValid = false, message = "CPF/CNPJ não pode ser vazio." });
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
