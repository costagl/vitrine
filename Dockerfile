# Estágio 1: Build (Construção)
# Usamos a imagem SDK para compilar o código
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo .csproj e restaura as dependências
# Isso aproveita o cache se as dependências não mudarem
COPY *.csproj .
RUN dotnet restore

# Copia todo o restante do código
COPY . .
# Publica (compila) o projeto para produção
RUN dotnet publish -c Release -o /app/publish

# -----------------------------------------------------------------

# Estágio 2: Final (Execução)
# Usamos a imagem ASPNET para rodar a aplicação (é menor e mais segura)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia apenas os arquivos de execução do estágio de build
COPY --from=build /app/publish .

# Define a porta que o contêiner irá expor (sua API)
EXPOSE 8080

# Comando que inicia a aplicação ao rodar o contêiner
ENTRYPOINT ["dotnet", "VitrineApi.dll"]