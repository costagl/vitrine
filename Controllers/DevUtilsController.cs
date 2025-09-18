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
        private VitrineDBContext _db;
        private readonly IRepositoryBase<CategoriaProduto> _repCategoria;

        public DevUtilsController(VitrineDBContext db, IRepositoryBase<CategoriaProduto> repCategoria)
        {
            _db = db;
            _repCategoria = repCategoria;
        }

        [HttpPost("adicionar-categorias")]
        public void AdicionarCategoriasGenericas()
        {
            if (_db.CategoriaProduto.Any())
            {
                Console.WriteLine("A tabela tem dados existentes.");
            }
            else
            {
                List<CategoriaProduto> categorias = new List<CategoriaProduto>
                {
                    new CategoriaProduto { Titulo = "Eletrônicos", Imagem = "eletronicos.jpg" },
                    new CategoriaProduto { Titulo = "Roupas", Imagem = "roupas.jpg" },
                    new CategoriaProduto { Titulo = "Alimentos", Imagem = "alimentos.jpg" },
                    new CategoriaProduto { Titulo = "Beleza", Imagem = "beleza.jpg" },
                    new CategoriaProduto { Titulo = "Saúde", Imagem = "saude.jpg" },
                    new CategoriaProduto { Titulo = "Brinquedos", Imagem = "brinquedos.jpg" },
                    new CategoriaProduto { Titulo = "Automóveis", Imagem = "automoveis.jpg" },
                    new CategoriaProduto { Titulo = "Móveis", Imagem = "moveis.jpg" },
                    new CategoriaProduto { Titulo = "Esportes", Imagem = "esportes.jpg" },
                    new CategoriaProduto { Titulo = "Livros", Imagem = "livros.jpg" }
                };

                _db.CategoriaProduto.AddRange(categorias);
                _db.SaveChanges();
            }
        }
    }
}