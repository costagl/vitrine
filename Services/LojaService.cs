using System;
using VitrineApi.Data;
using VitrineApi.Interfaces;
using VitrineApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace VitrineApi.Services
{
    public class LojaService : ILojaService
    {
        private readonly VitrineDBContext _context;

        public LojaService(VitrineDBContext context)
        {
            _context = context;
        }

        public async Task<LojaRequest> BuscarPorSubdominio(string subdominio)
        {
            var loja = await _context.Loja
                .Where(l => l.Subdominio == subdominio)
                .Select(l => new LojaRequest
                {
                    Id = l.Id,
                    NomeLoja = l.NomeLoja,
                    CategoriaLoja = l.IdCategoriaNavigation.Titulo,
                    IdTema = l.IdTema,
                    IdLayout = l.IdLayout,
                    Subdominio = l.Subdominio,
                    Cpf_Cnpj = l.Cpf_Cnpj,
                })
                .FirstOrDefaultAsync();

            return loja;
        }
    }
}