FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Calculator.Kafka.Worker/Calculator.Kafka.Worker.csproj", "Calculator.Kafka.Worker/"]
COPY ["Calculator.Kafka.Infrastructure/Calculator.Kafka.Infrastructure.csproj", "Calculator.Kafka.Infrastructure/"]
COPY ["Calculator.Kafka.Models/Calculator.Kafka.Models.csproj", "Calculator.Kafka.Models/"]
RUN dotnet restore "Calculator.Kafka.Worker/Calculator.Kafka.Worker.csproj"
COPY . .
WORKDIR "/src/Calculator.Kafka.Worker"
RUN dotnet build "Calculator.Kafka.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Calculator.Kafka.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Calculator.Kafka.Worker.dll"]