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
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<LojistaAuth> _signInManager;
        private readonly UserManager<LojistaAuth> _userManager;
        private readonly IConfiguration _config;
        private readonly VitrineDBContext _context;

        public AccountController(SignInManager<LojistaAuth> signInManager, UserManager<LojistaAuth> userManager, IConfiguration config, VitrineDBContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
            _context = context;
        }

        [HttpGet("test")]
        public IActionResult Test()
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
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                var token = GenerateJwtToken(user);
                return Ok(new { token });
            }

            return Unauthorized(new { message = "E-mail ou senha inválidos." });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Lojista.Any(l => l.Cpf == model.Cpf))
                return BadRequest("CPF já cadastrado.");

            var lojista = new Lojista
            {
                NomeCompleto = model.Name,
                Celular = model.Celular,
                Cpf = model.Cpf,
                DataNascimento = DateOnly.ParseExact(model.DataNascimento, "dd/MM/yyyy", null),
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

            var result = await _userManager.CreateAsync(userAuth, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var loja = new Loja
            {
                NomeLoja = model.NomeLoja,
                CategoriaLoja = model.CategoriaVenda,
                Tema = "Padrão",
                Layout = "Clássico",
                Cpf = model.Cpf,
                Cnpj = model.Cnpj
            };

            _context.Loja.Add(loja);
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(userAuth, isPersistent: false);

            return Ok(new { message = "Cadastro realizado com sucesso!" });
        }



        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout bem-sucedido!" });
        }

        [HttpGet("register-test")]
        public async Task<IActionResult> RodarRegisterDireto()
        {
            return await RegisterTest(); 
        }

        [HttpPost("register-test")]
        public async Task<IActionResult> RegisterTest()
        {
            string cpfAux = "11111111115";

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_context.Lojista.Any(l => l.Cpf == cpfAux))
                return BadRequest("CPF já cadastrado.");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var lojista = new Lojista
                {
                    NomeCompleto = "Register Test",
                    Celular = "24999642650",
                    Cpf = cpfAux,
                    DataNascimento = DateOnly.ParseExact("14/12/2002", "dd/MM/yyyy", null),
                    Email = "registerTest@gmail.com"
                };

                await _context.Lojista.AddAsync(lojista);
                await _context.SaveChangesAsync();

                var userAuth = new LojistaAuth
                {
                    UserName = "registerTest@gmail.com",
                    Email = "registerTest@gmail.com",
                    Cpf = cpfAux
                };

                var result = await _userManager.CreateAsync(userAuth, "1234567890");
                if (!result.Succeeded)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(result.Errors);
                }

                var loja = new Loja
                {
                    NomeLoja = "Loja Register Test",
                    CategoriaLoja = "Categoria Register Test",
                    Tema = "Padrão",
                    Layout = "Clássico",
                    Cpf = cpfAux,
                    Cnpj = "11111111111111"
                };

                _context.Loja.Add(loja);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                await _signInManager.SignInAsync(userAuth, isPersistent: false);

                return Ok(new { message = "Cadastro realizado com sucesso!" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { message = "Erro ao cadastrar: " + ex.Message });
            }
        }

    }
}
