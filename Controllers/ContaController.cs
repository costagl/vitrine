using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using VitrineApi.Data;
using VitrineApi.Models;
using VitrineApi.ViewModels;

namespace VitrineApi.Controllers
{
    [Route("usuario")]
    [ApiController]
    public class ContaController : Controller
    {
        private readonly SignInManager<LojistaAuth> _signInManager;
        private readonly UserManager<LojistaAuth> _userManager;
        private readonly IConfiguration _config;
        private readonly VitrineDBContext _context;

        public ContaController(SignInManager<LojistaAuth> signInManager, UserManager<LojistaAuth> userManager, IConfiguration config, VitrineDBContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _context = context;
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
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
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
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Senha,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                var loja = await _context.Loja.FirstOrDefaultAsync(l => l.Cpf == user.Cpf);

                var token = GenerateJwtToken(user);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = user.Id,
                        npme = user.UserName,
                        email = user.Email,
                        loja = loja == null ? null : new
                        {
                            id = loja.Id,
                            nome = loja.NomeLoja,
                            categoria = loja.CategoriaLoja,
                            subdominio = loja.Subdominio
                        }
                    }
                });
            }

            return Unauthorized(new { message = "E-mail ou senha inválidos." });
        }



        [HttpPost("cadastrar")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Lojista.Any(l => l.Cpf == model.Cpf))
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
                    Cpf = model.Cpf,
                    DataNascimento = dataNascimento,
                    Email = model.Email
                };


                _context.Lojista.Add(lojista);
                await _context.SaveChangesAsync();

                var userAuth = new LojistaAuth
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Cpf = model.Cpf
                };

                var result = await _userManager.CreateAsync(userAuth, model.Senha);
                if (!result.Succeeded)
                    return BadRequest(result.Errors); // A transação vai ser descartada no `finally` se não der commit

                var loja = new Loja
                {
                    NomeLoja = model.NomeLoja,
                    CategoriaLoja = model.CategoriaVenda,
                    IdTema = 1,
                    IdLayout = 1,
                    Cpf = model.Cpf,
                    Cnpj = model.Cnpj,
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


        [HttpPost("verificar-subdominio")]
        public async Task<IActionResult> VerificarSubdominioExistente([FromBody] SubdominioRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Subdominio))
                return BadRequest(new { message = "Subdomínio não pode ser vazio" });

            bool existe = (await new RepositoryBase<Loja>(_context).ListarAsync(
                c => c.Subdominio.ToLower() == request.Subdominio.ToLower())).Any();

            return Ok(new { disponivel = !existe });
        }
    }
}
