FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Dragon.ApiGateway/Dragon.ApiGateway.csproj", "src/Dragon.ApiGateway/"]
RUN dotnet restore "src/Dragon.ApiGateway/Dragon.ApiGateway.csproj"

COPY . .
RUN dotnet publish "src/Dragon.ApiGateway/Dragon.ApiGateway.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Dragon.ApiGateway.dll"]
