using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Interfaces;
using VitrineApi.Models;

namespace VitrineApi.Services
{
    public class ProdutoService
    {
        private readonly VitrineDBContext _context;
        private readonly IMapper _mapper;

        public ProdutoService(VitrineDBContext context)
        {
            _context = context;
        }

        public async Task<List<ProdutoDTO>> ObterProdutosPorLojaAsync(int idLoja)
        {
            // Não precisamos mais buscar a Loja antes, pois já temos o ID
            // Se quiser validar se a loja existe, pode fazer, mas a query abaixo 
            // simplesmente retornará vazio se o ID não existir, o que é performático.

            var produtosData = await _context.Produto
                .Where(p => p.IdLoja == idLoja) // Filtro direto pelo parâmetro
                .Join(
                    _context.CategoriaProduto,
                    produto => produto.IdCategoriaProduto,
                    categoria => categoria.Id,
                    (produto, categoria) => new { produto, categoria }
                )
                // Mantemos o Join com Loja aqui para poder pegar o IdCategoria da loja
                // e consequentemente o Nome da Categoria da Loja
                .Join(
                    _context.Loja,
                    produtoCategoria => produtoCategoria.produto.IdLoja,
                    loja => loja.Id,
                    (produtoCategoria, loja) => new { produtoCategoria.produto, produtoCategoria.categoria, loja }
                )
                .Join(
                    _context.CategoriaLoja,
                    produtoLoja => produtoLoja.loja.IdCategoria,
                    categoriaLoja => categoriaLoja.Id,
                    (produtoLoja, categoriaLoja) => new ProdutoDTO
                    {
                        Id = produtoLoja.produto.Id,
                        Titulo = produtoLoja.produto.Titulo,
                        IdLoja = produtoLoja.produto.IdLoja,
                        ValorUnitario = produtoLoja.produto.ValorUnitario,
                        ValorPromocional = produtoLoja.produto.ValorPromocional,
                        Estoque = produtoLoja.produto.Estoque, // Confirme se no banco é 'Estoque' ou 'Quantidade'
                        Sku = produtoLoja.produto.Sku,
                        Imagem = produtoLoja.produto.ImagemUrl,
                        Ativo = produtoLoja.produto.Ativo,
                        Peso = produtoLoja.produto.Peso,
                        Descricao = produtoLoja.produto.Descricao,
                        Altura = produtoLoja.produto.Altura,
                        Largura = produtoLoja.produto.Largura,
                        Profundidade = produtoLoja.produto.Profundidade,
                        IdCategoriaProduto = produtoLoja.produto.IdCategoriaProduto,
                        TituloCategoriaProduto = produtoLoja.categoria.Titulo,
                        IdCategoriaLoja = produtoLoja.loja.IdCategoria,
                        TituloCategoriaLoja = categoriaLoja.Titulo
                    })
                .ToListAsync();

            return produtosData;
        }

        public async Task<ProdutoDTO?> ObterDetalhesProdutoAsync(int id)
        {
            var produtoData = await _context.Produto
                .Where(p => p.Id == id)
                .Include(p => p.IdCategoriaProdutoNavigation)
                .Include(p => p.IdLojaNavigation)
                .ThenInclude(l => l.IdCategoriaNavigation)
                .Select(p => new ProdutoDTO
                {
                    Id = p.Id,
                    Titulo = p.Titulo,
                    IdLoja = p.IdLoja,
                    ValorUnitario = p.ValorUnitario,
                    ValorPromocional = p.ValorPromocional,
                    Estoque = p.Estoque,
                    Sku = p.Sku,
                    Imagem = p.ImagemUrl,
                    Ativo = p.Ativo,
                    Peso = p.Peso,
                    Descricao = p.Descricao,
                    Altura = p.Altura,
                    Largura = p.Largura,
                    Profundidade = p.Profundidade,
                    IdCategoriaProduto = p.IdCategoriaProduto,
                    TituloCategoriaProduto = p.IdCategoriaProdutoNavigation.Titulo,
                    IdCategoriaLoja = p.IdLojaNavigation.IdCategoria,
                    TituloCategoriaLoja = p.IdLojaNavigation.IdCategoriaNavigation.Titulo
                })
                .FirstOrDefaultAsync();

            return produtoData;
        }


    }
}
