using VitrineApi.DTOs;

namespace VitrineApi.Interfaces;

public interface ILojaService
{
    Task<LojaDto> BuscarPorSubdominio(string subdominio);
}