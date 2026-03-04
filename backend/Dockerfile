FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ErpDemo.sln .
COPY src/ErpDemo.Domain/ErpDemo.Domain.csproj src/ErpDemo.Domain/
COPY src/ErpDemo.Application/ErpDemo.Application.csproj src/ErpDemo.Application/
COPY src/ErpDemo.Infrastructure/ErpDemo.Infrastructure.csproj src/ErpDemo.Infrastructure/
COPY src/ErpDemo.Api/ErpDemo.Api.csproj src/ErpDemo.Api/
COPY tests/ErpDemo.Tests/ErpDemo.Tests.csproj tests/ErpDemo.Tests/

RUN dotnet restore

COPY . .
RUN dotnet publish src/ErpDemo.Api/ErpDemo.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000
ENTRYPOINT ["dotnet", "ErpDemo.Api.dll"]
