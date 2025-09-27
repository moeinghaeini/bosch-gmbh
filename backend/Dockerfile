FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BoschThesis.API/BoschThesis.API.csproj", "BoschThesis.API/"]
COPY ["BoschThesis.Core/BoschThesis.Core.csproj", "BoschThesis.Core/"]
COPY ["BoschThesis.Infrastructure/BoschThesis.Infrastructure.csproj", "BoschThesis.Infrastructure/"]
RUN dotnet restore "BoschThesis.API/BoschThesis.API.csproj"
COPY . .
WORKDIR "/src/BoschThesis.API"
RUN dotnet build "BoschThesis.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BoschThesis.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BoschThesis.API.dll"]
