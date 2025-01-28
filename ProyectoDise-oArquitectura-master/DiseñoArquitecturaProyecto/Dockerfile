# Usar la imagen base de ASP.NET Core con .NET 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Usar la imagen del SDK de .NET para construir el proyecto
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar el archivo .csproj al contenedor para restaurar dependencias
COPY ["DiseñoArquitecturaProyecto/DiseñoArquitecturaProyecto.csproj", "DiseñoArquitecturaProyecto/"]
RUN dotnet restore "DiseñoArquitecturaProyecto/DiseñoArquitecturaProyecto.csproj"

# Copiar el resto de los archivos del proyecto y construir
COPY . .
WORKDIR "/src/DiseñoArquitecturaProyecto"
RUN dotnet build "DiseñoArquitecturaProyecto.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicar la aplicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiseñoArquitecturaProyecto.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final con los archivos publicados
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiseñoArquitecturaProyecto.dll"]
