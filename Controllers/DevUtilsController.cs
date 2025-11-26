using Microsoft.AspNetCore.Mvc;
using VitrineApi.Data;
using VitrineApi.Helpers;
using VitrineApi.Models;

namespace DevUtils
{
    [ApiController]
    [Route("[controller]")]
    public class DevUtilsController : Controller
    {
        private VitrineDBContext _context;
        private readonly DbEsgotado _dbEsgotado;


        public DevUtilsController(VitrineDBContext context, DbEsgotado dbEsgotado)
        {
            _context = context;
            _dbEsgotado = dbEsgotado;
        }

        [HttpPost("adicionar-categorias-produto")]
        public async Task<IActionResult> AdicionarCategoriasProduto()
        {
            if (_dbEsgotado.VerificarBancoEsgotado())
            {
                return StatusCode(500, new { message = "Banco de dados esgotado." });
            }

            if (_context.CategoriaProduto.Any())
            {
                return Conflict(new { message = "A tabela já contém dados." });
            }
            else
            {
                var categorias = new List<List<string>>
        {
            // Eletrônicos
            new List<string> { "Celulares", "Notebooks", "Televisores", "Fones de Ouvido", "Câmeras Digitais",
                               "Tablets", "Smartwatches", "Consoles de Videogame", "Drones", "Acessórios Eletrônicos" },
            
            // Roupas
            new List<string> { "Camisetas", "Calças", "Vestidos", "Blusas", "Shorts", "Jaquetas", "Moda Íntima",
                               "Roupas Esportivas", "Moda Praia", "Acessórios de Moda" },

            // Alimentos
            new List<string> { "Grãos e Cereais", "Laticínios", "Carnes", "Frutas e Verduras", "Bebidas",
                               "Doces e Sobremesas", "Produtos Orgânicos", "Congelados", "Massas e Molhos", "Temperos e Especiarias" },

            // Beleza
            new List<string> { "Maquiagem", "Perfumes", "Cuidados com a Pele", "Cuidados com o Cabelo", "Esmaltes",
                               "Hidratantes", "Protetores Solares", "Kits de Beleza", "Produtos Naturais", "Acessórios de Beleza" },

            // Saúde
            new List<string> { "Suplementos", "Vitaminas", "Medicamentos", "Produtos Naturais", "Equipamentos Médicos",
                               "Primeiros Socorros", "Cuidados Pessoais", "Higiene Bucal", "Saúde Feminina", "Bem-Estar" },

            // Brinquedos
            new List<string> { "Bonecos e Bonecas", "Jogos de Tabuleiro", "Brinquedos Educativos", "Carrinhos", "Lego e Blocos de Montar",
                               "Pelúcias", "Instrumentos Musicais Infantis", "Brinquedos de Exterior", "Brinquedos Eletrônicos", "Fantasias" },

            // Automóveis
            new List<string> { "Peças Automotivas", "Acessórios Internos", "Acessórios Externos", "Som Automotivo", "Lubrificantes",
                               "Pneus e Rodas", "Equipamentos de Segurança", "Limpeza Automotiva", "Ferramentas", "Motocicletas e Acessórios" },

            // Móveis
            new List<string> { "Sofás", "Camas", "Mesas", "Cadeiras", "Armários", "Racks e Painéis", "Escrivaninhas", "Poltronas",
                               "Móveis de Cozinha", "Móveis para Escritório" },

            // Esportes
            new List<string> { "Roupas Esportivas", "Calçados Esportivos", "Equipamentos de Academia", "Bolas", "Acessórios Fitness",
                               "Esportes ao Ar Livre", "Ciclismo", "Natação", "Artes Marciais", "Suplementos Esportivos" },

            // Livros
            new List<string> { "Romance", "Ficção Científica", "Biografias", "Autoajuda", "Didáticos", "Negócios e Economia",
                               "Religião e Espiritualidade", "Infantis", "HQs e Mangás", "Literatura Brasileira" }
        };
                var categoriasProduto = new List<CategoriaProduto>();
                int idCatLoja = 1;
                int idCatProd = 1;

                foreach (var categoria in categorias)
                {
                    foreach (var subcategoria in categoria)
                    {
                        categoriasProduto.Add(new CategoriaProduto { Id = idCatProd, Titulo = subcategoria, IdCategoriaLoja = idCatLoja });
                        idCatProd++;
                    }
                    idCatLoja++;
                }

                _context.CategoriaProduto.AddRange(categoriasProduto);
                _context.SaveChanges();
                return Ok(new { message = "Dados adicionados." });
            }
        }

        [HttpPost("adicionar-categorias-loja")]
        public async Task<IActionResult> AdicionarCategoriasLoja()
        {
            if (_dbEsgotado.VerificarBancoEsgotado())
            {
                return StatusCode(500, new { message = "Banco de dados esgotado." });
            }

            if (_context.CategoriaLoja.Any())
            {
                return Conflict(new { message = "A tabela já contém dados." });
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
                return Ok(new { message = "Dados adicionados." });
            }
        }

        [HttpPost("adicionar-layout-tema")]
        public async Task<IActionResult> AdicionarLayoutTema()
        {
            if (_dbEsgotado.VerificarBancoEsgotado())
            {
                return StatusCode(500, new { message = "Banco de dados esgotado." });
            }

            if (_context.Layout.Any() || _context.Tema.Any())
            {
                return Conflict(new { message = "A tabela já contém dados." });
            }
            else
            {
                List<Layout> layouts = new List<Layout>
        {
            new Layout { Titulo = "layout-1", Descricao = "Layout sofisticado para moda e acessórios" },
            new Layout { Titulo = "layout-2", Descricao = "Design clean e moderno para produtos tecnológicos" },
            new Layout { Titulo = "layout-3", Descricao = "Design fofo para peças de biscuit e bolos cenográficos" },
        };

                List<Tema> temas = new List<Tema>
        {
            new Tema
            {
                Titulo = "Azul Profissional",
                CorPrimaria = "#2563eb",
                CorSecundaria = "#64748b",
                Realce = "#0ea5e9"
            },
            new Tema
            {
                Titulo = "Roxo Moderno",
                CorPrimaria = "#4400FF",
                CorSecundaria = "#8b5cf6",
                Realce = "#a855f7"
            },
            new Tema
            {
                Titulo = "Verde Natural",
                CorPrimaria = "#059669",
                CorSecundaria = "#10b981",
                Realce = "#34d399"
            },
            new Tema
            {
                Titulo = "Laranja Energético",
                CorPrimaria = "#ea580c",
                CorSecundaria = "#f97316",
                Realce = "#fb923c"
            },
            new Tema
            {
                Titulo = "Rosa Criativo",
                CorPrimaria = "#db2777",
                CorSecundaria = "#ec4899",
                Realce = "#f472b6"
            },
            new Tema
            {
                Titulo = "Azul Turquesa",
                CorPrimaria = "#0d9488",
                CorSecundaria = "#14b8a6",
                Realce = "#2dd4bf"
            }
        };

                _context.Layout.AddRange(layouts);
                _context.Tema.AddRange(temas);
                _context.SaveChanges();
                return Ok(new { message = "Dados adicionados." });
            }
        }

    }
}