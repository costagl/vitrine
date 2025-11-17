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

    public CategoriaController(IRepositoryBase<CategoriaProduto> catProdRep, IRepositoryBase<CategoriaLoja> catLojaRep)
    {
        _CatProdRep = catProdRep;
        _CatLojaRep = catLojaRep;
    }

    [AllowAnonymous]
    [HttpGet("produto")]
    public async Task<IActionResult> ListarCategoriasProduto()
    {
        var categoriaProduto = await _CatProdRep.ListarAsync();
        return Ok(categoriaProduto);
    }

    [AllowAnonymous]
    [HttpGet("loja")]
    public async Task<IActionResult> ListarCategoriasLoja()
    {
        var categoriaLoja = await _CatLojaRep.ListarAsync();
        return Ok(categoriaLoja);
    }
}
