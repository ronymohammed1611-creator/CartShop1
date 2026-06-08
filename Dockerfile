FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY CartShop.sln .
COPY CartShop/CartShop.csproj CartShop/
COPY CartShop.BLL/CartShop.BLL.csproj CartShop.BLL/
COPY CartShop.DAL/CartShop.DAL.csproj CartShop.DAL/

# Restore dependencies
RUN dotnet restore

# Copy everything and build
COPY . .
RUN dotnet publish CartShop/CartShop.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}
ENTRYPOINT ["dotnet", "CartShop.dll"]
