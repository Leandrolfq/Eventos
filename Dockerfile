# Estágio de compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copiar tudo e restaurar as dependências
COPY . ./
RUN dotnet restore

# Compilar a aplicação em modo Release
RUN dotnet publish -c Release -o out

# Estágio de execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Configurar a porta que o Render vai usar
ENV ASPNETCORE_URLS=http://+:10000

ENTRYPOINT ["dotnet", "Eventos.dll"]