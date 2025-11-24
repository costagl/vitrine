using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VitrineApi.Data;
using VitrineApi.DTOs;
using VitrineApi.Interfaces;
using VitrineApi.Models;
using VitrineApi.Mappings;
using AutoMapper;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("categoria")]
public class CategoriaController : ControllerBase
{
    private readonly IRepositoryBase<CategoriaProduto> _CatProdRep;
    private readonly IRepositoryBase<CategoriaLoja> _CatLojaRep;
    private readonly VitrineDBContext _context;

    public CategoriaController(IRepositoryBase<CategoriaProduto> catProdRep, IRepositoryBase<CategoriaLoja> catLojaRep, VitrineDBContext context)
    {
        _CatProdRep = catProdRep;
        _CatLojaRep = catLojaRep;
        _context = context;
    }

    [AllowAnonymous]
    [HttpGet("loja")]
    public async Task<IActionResult> ListarCategoriasLoja()
    {
        var categoriaLoja = await _CatLojaRep.ListarAsync();
        return Ok(categoriaLoja);
    }

    [AllowAnonymous]
    [HttpGet("produtos/{id}")]
    public async Task<IActionResult> ListarCategoriasProduto(int id)
    {
        var categoriaProduto = await _CatProdRep.ListarAsync(c => c.IdCategoriaLoja == id);
        return Ok(categoriaProduto);
    }
}
