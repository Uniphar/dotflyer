FROM mcr.microsoft.com/dotnet/sdk:8.0 AS sdk
WORKDIR /src
COPY ["DotFlyer.Api/DotFlyer.Api.csproj", "DotFlyer.Api/"]
RUN dotnet restore "./DotFlyer.Api/DotFlyer.Api.csproj"
COPY . .
WORKDIR "/src/DotFlyer.Api"
RUN dotnet publish "./DotFlyer.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
USER app
WORKDIR /app
EXPOSE 8080
COPY --from=sdk /app/publish .
ENTRYPOINT ["dotnet", "DotFlyer.Api.dll"]