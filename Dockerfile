# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Tối ưu hóa Restore layer
COPY ["src/Dragon.ApiGateway/Dragon.ApiGateway.csproj", "src/Dragon.ApiGateway/"]
RUN dotnet restore "src/Dragon.ApiGateway/Dragon.ApiGateway.csproj"

COPY . .
WORKDIR "/src/src/Dragon.ApiGateway"
RUN dotnet publish "Dragon.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime (Chiseled - Không Shell, Không Root, Siêu bảo mật)
FROM mcr.microsoft.com/dotnet/nightly/aspnet:8.0-noble-chiseled AS final
WORKDIR /app
COPY --from=build --chown=app:app /app/publish .

# Cấu hình Port chuẩn cho Non-root user
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

USER app
ENTRYPOINT ["dotnet", "Dragon.ApiGateway.dll"]
