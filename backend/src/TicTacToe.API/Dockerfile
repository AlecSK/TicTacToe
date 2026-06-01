FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/TicTacToe.API/TicTacToe.API.csproj", "src/TicTacToe.API/"]
COPY ["src/TicTacToe.Application/TicTacToe.Application.csproj", "src/TicTacToe.Application/"]
COPY ["src/TicTacToe.Domain/TicTacToe.Domain.csproj", "src/TicTacToe.Domain/"]
COPY ["src/TicTacToe.Infrastructure/TicTacToe.Infrastructure.csproj", "src/TicTacToe.Infrastructure/"]
COPY ["Directory.Build.props", "."]

RUN dotnet restore "src/TicTacToe.API/TicTacToe.API.csproj"

COPY . .
RUN dotnet publish "src/TicTacToe.API/TicTacToe.API.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "TicTacToe.API.dll"]
