# Dockerfile para BancoAPI

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["BancoAPI.API/BancoAPI.API.csproj", "BancoAPI.API/"]
COPY ["BancoAPI.Application/BancoAPI.Application.csproj", "BancoAPI.Application/"]
COPY ["BancoAPI.Infrastructure/BancoAPI.Infrastructure.csproj", "BancoAPI.Infrastructure/"]
COPY ["BancoAPI.Domain/BancoAPI.Domain.csproj", "BancoAPI.Domain/"]

RUN dotnet restore "BancoAPI.API/BancoAPI.API.csproj"

# Copiar todo el código fuente
COPY . .

# Build de la aplicación
WORKDIR "/src/BancoAPI.API"
RUN dotnet build "BancoAPI.API.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "BancoAPI.API.csproj" -c Release -o /app/publish

# Imagen final de runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Configurar zona horaria de Ecuador (UTC-5)
ENV TZ=America/Guayaquil
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Copiar la aplicación publicada
COPY --from=publish /app/publish .

# Punto de entrada
ENTRYPOINT ["dotnet", "BancoAPI.API.dll"]
