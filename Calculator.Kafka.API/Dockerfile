FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Calculator.Kafka.API/Calculator.Kafka.API.csproj", "Calculator.Kafka.API/"]
COPY ["Calculator.Kafka.Infrastructure/Calculator.Kafka.Infrastructure.csproj", "Calculator.Kafka.Infrastructure/"]
COPY ["Calculator.Kafka.Models/Calculator.Kafka.Models.csproj", "Calculator.Kafka.Models/"]
RUN dotnet restore "Calculator.Kafka.API/Calculator.Kafka.API.csproj"
COPY . .
WORKDIR "/src/Calculator.Kafka.API"
RUN dotnet build "Calculator.Kafka.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Calculator.Kafka.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Calculator.Kafka.API.dll"]