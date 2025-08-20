FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BarberShopAPI.Server.csproj", "BarberShopAPI.Server/"]
RUN dotnet restore "BarberShopAPI.Server/BarberShopAPI.Server.csproj"
COPY . .
WORKDIR "/src/BarberShopAPI.Server"
RUN dotnet build "BarberShopAPI.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BarberShopAPI.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BarberShopAPI.Server.dll"]