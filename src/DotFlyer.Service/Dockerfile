FROM mcr.microsoft.com/dotnet/sdk:8.0 AS sdk
WORKDIR /src
COPY ["DotFlyer.Service/DotFlyer.Service.csproj", "DotFlyer.Service/"]
RUN dotnet restore "./DotFlyer.Service/DotFlyer.Service.csproj"
COPY . .
WORKDIR "/src/DotFlyer.Service"
RUN dotnet publish "./DotFlyer.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
USER app
WORKDIR /app
EXPOSE 8080
COPY --from=sdk /app/publish .
ENTRYPOINT ["dotnet", "DotFlyer.Service.dll"]