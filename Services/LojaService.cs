using System;
using VitrineApi.Data;
using VitrineApi.Interfaces;
using VitrineApi.Dtos;
using Microsoft.EntityFrameworkCore;

public class LojaService : ILojaService
{
    private readonly VitrineDBContext _context;

    public LojaService(VitrineDBContext context)
    {
        _context = context;
    }

    public async Task<LojaDto> BuscarPorSubdominio(string subdominio)
    {
        var loja = await _context.Loja
            .Where(l => l.Subdominio == subdominio)
            .Select(l => new LojaDto
            {
                Id = l.Id,
                NomeLoja = l.NomeLoja,
                CategoriaLoja = l.CategoriaLoja,
                Tema = l.Tema,
                Layout = l.Layout,
                Subdominio = l.Subdominio,
                Cpf = l.Cpf,
                Cnpj = l.Cnpj
            })
            .FirstOrDefaultAsync();

        return loja;
    }
}
