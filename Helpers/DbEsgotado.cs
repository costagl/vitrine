using VitrineApi.Data;
using Microsoft.EntityFrameworkCore;

namespace VitrineApi.Helpers
{
    public class DbEsgotado
    {
        private readonly VitrineDBContext _context;

        public DbEsgotado(VitrineDBContext context)
        {
            _context = context;
        }

        int valor = 1000;
        public bool VerificarBancoEsgotado()
        {
            if (_context.CategoriaProduto.Count() > valor)
                return true;
            if (_context.CategoriaLoja.Count() > valor)
                return true;
            if (_context.Cliente.Count() > valor)
                return true;
            if (_context.EnderecoEntrega.Count() > valor)
                return true;
            if (_context.ItensPedido.Count() > valor)
                return true;
            if (_context.Layout.Count() > valor)
                return true;
            if (_context.Loja.Count() > valor)
                return true;
            if (_context.Lojista.Count() > valor)
                return true;
            if (_context.Pedido.Count() > valor)
                return true;
            if (_context.Produto.Count() > valor)
                return true;
            if (_context.Tema.Count() > valor)
                return true;

            return false;
        }
    }
}
