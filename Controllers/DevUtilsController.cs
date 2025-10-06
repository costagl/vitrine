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
using VitrineApi.Interfaces;
using VitrineApi.Models;
using VitrineApi.ViewModels;

namespace DevUtils
{
    [ApiController]
    [Route("[controller]")]
    public class DevUtilsController : Controller
    {
        private VitrineDBContext _context;

        public DevUtilsController(VitrineDBContext context)
        {
            _context = context;
        }

        [HttpPost("adicionar-categorias-produto")]
        public void AdicionarCategoriasProduto()
        {
            if (_context.CategoriaLoja.Any())
            {
                Console.WriteLine("A tabela tem dados existentes.");
            }
            else
            {
                List<CategoriaProduto> categorias = new List<CategoriaProduto>
                {
                    new CategoriaProduto { Titulo = "Eletrônicos" },
                    new CategoriaProduto { Titulo = "Roupas" },
                    new CategoriaProduto { Titulo = "Alimentos" },
                    new CategoriaProduto { Titulo = "Beleza" },
                    new CategoriaProduto { Titulo = "Saúde" },
                    new CategoriaProduto { Titulo = "Brinquedos" },
                    new CategoriaProduto { Titulo = "Automóveis" },
                    new CategoriaProduto { Titulo = "Móveis" },
                    new CategoriaProduto { Titulo = "Esportes" },
                    new CategoriaProduto { Titulo = "Livros" }
                };

                _context.CategoriaProduto.AddRange(categorias);
                _context.SaveChanges();
            }
        }

        [HttpPost("adicionar-categorias-loja")]
        public void AdicionarCategoriasLoja()
        {
            if (_context.CategoriaLoja.Any())
            {
                Console.WriteLine("A tabela tem dados existentes.");
            }
            else
            {
                List<CategoriaLoja> categorias = new List<CategoriaLoja>
                {
                    new CategoriaLoja { Titulo = "Eletrônicos" },
                    new CategoriaLoja { Titulo = "Roupas" },
                    new CategoriaLoja { Titulo = "Alimentos" },
                    new CategoriaLoja { Titulo = "Beleza" },
                    new CategoriaLoja { Titulo = "Saúde" },
                    new CategoriaLoja { Titulo = "Brinquedos" },
                    new CategoriaLoja { Titulo = "Automóveis" },
                    new CategoriaLoja { Titulo = "Móveis" },
                    new CategoriaLoja { Titulo = "Esportes" },
                    new CategoriaLoja { Titulo = "Livros" }
                };

                var oCat = new RepositoryBase<CategoriaLoja>(_context);

                oCat.IncluirListaAsync(categorias);
            }
        }

        [HttpPost("adicionar-layout-tema")]
        public void AdicionarLayoutTema()
        {
            if (_context.Layout.Any() || _context.Tema.Any())
            {
                Console.WriteLine("As tabelas tem dados existentes.");
            }
            else
            {
                List<Layout> layouts = new List<Layout>
                {
                    new Layout { Nome = "layout-1" },
                    new Layout { Nome = "layout-2" },
                    new Layout { Nome = "layout-3" },
                    new Layout { Nome = "layout-4" },
                };
                List<Tema> temas = new List<Tema>
                {
                    new Tema { Nome = "tema-1" },
                    new Tema { Nome = "tema-2" },
                    new Tema { Nome = "tema-3" },
                    new Tema { Nome = "tema-4" },
                };

                _context.Layout.AddRange(layouts);
                _context.Tema.AddRange(temas);
                _context.SaveChanges();
            }
        }
    }
}