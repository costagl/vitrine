using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Vitrine.Models;
using VitrineApi.ViewModels;

namespace Vitrine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly SignInManager<Lojista> _signInManager;
        private readonly UserManager<Lojista> _userManager;
        private readonly IConfiguration _config;

        public AccountController(SignInManager<Lojista> signInManager, UserManager<Lojista> userManager, IConfiguration config)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
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
            if (ModelState.IsValid)
            {
                var user = new Lojista
                {
                    NomeCompleto = model.Name,
                    Email = model.Email,
                    UserName = model.Email,
                    CPF = model.CPF,
                    DataNascimento = DateTime.Parse(model.DataNascimento),
                    CNPJ = model.CNPJ,
                    NomeLoja = model.NomeLoja,
                    CategoriaVenda = model.CategoriaVenda,
                    LembrarDeMim = model.LembrarDeMim
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return Ok(new { message = "Cadastro realizado com sucesso!" });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }

            return BadRequest(ModelState); // Retorna erro de validação
        }

        //[HttpGet("register-test")]
        //public async Task<IActionResult> RodarRegisterDireto()
        //{
        //    return await RegisterTest(); // chama o método direto
        //}

        //[HttpPost("testar-cadastro")]
        //public async Task<IActionResult> RegisterTest()
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = new Lojista
        //        {
        //            NomeCompleto = "Gabriel",
        //            Email = "gabriel@gmail.com",
        //            UserName = "gabriel@gmail.com",

        //            CPF = "17873426700",
        //            DataNascimento = DateTime.Parse("14/12/2002"),
        //            CNPJ = "12345678901234",
        //            NomeLoja = "Nome Loja",
        //            CategoriaVenda = "Categoria Venda",
        //            LembrarDeMim = false
        //        };

        //        var result = await _userManager.CreateAsync(user, "12345678");
        //        if (result.Succeeded)
        //        {
        //            await _signInManager.SignInAsync(user, isPersistent: false);
        //            return Ok(new { message = "Cadastro realizado com sucesso!" });
        //        }
        //        else
        //        {
        //            return BadRequest(result.Errors);
        //        }
        //    }

        //    return BadRequest(ModelState); // Retorna erro de validação
        //}

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok(new { message = "Logout bem-sucedido!" });
        }
    }
}
