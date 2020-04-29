
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["VehicleDamage_FrontEnd/VehicleDamage_FrontEnd.csproj", "VehicleDamage_FrontEnd/"]
RUN dotnet restore "VehicleDamage_FrontEnd/VehicleDamage_FrontEnd.csproj"
COPY . .
WORKDIR "/src/VehicleDamage_FrontEnd"
RUN dotnet build "VehicleDamage_FrontEnd.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VehicleDamage_FrontEnd.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VehicleDamage_FrontEnd.dll"]
