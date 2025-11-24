using VitrineApi.DTOs;

namespace VitrineApi.Interfaces;

public interface ILojaService
{
    Task<LojaRequest> BuscarPorSubdominio(string subdominio);
}