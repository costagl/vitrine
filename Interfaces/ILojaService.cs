using VitrineApi.DTOs;

namespace VitrineApi.Interfaces;

public interface ILojaService
{
    Task<LojaDTO> BuscarPorSubdominio(string subdominio);
}