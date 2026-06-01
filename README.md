# TicTacToe

Игра крестики-нолики в браузере: игрок против AI (minimax). Angular 21 + ASP.NET Core 8 + PostgreSQL.

## Быстрый старт (Docker)

**Требования:** Docker Desktop

```bash
docker compose up --build
```

| Сервис | URL |
|---|---|
| Игра | http://localhost:4200 |
| Swagger (API) | http://localhost:5000/swagger |

Первый запуск скачивает образы и собирает проект — занимает несколько минут.  
Повторные запуски без `--build` стартуют мгновенно.

Остановить и удалить данные:

```bash
docker compose down -v
```

---

## Локальная разработка

### Требования

| Инструмент | Версия |
|---|---|
| .NET SDK | 8.0+ |
| Node.js | 22+ |
| npm | 11+ |
| PostgreSQL | 17 |

### 1. База данных

Запустить только PostgreSQL из Docker Compose:

```bash
docker compose up postgres -d
```

Или использовать локальную установку PostgreSQL с параметрами:

```
Host=localhost  Port=5432  Database=tictactoe  User=postgres  Password=postgres
```

### 2. Backend (Visual Studio)

Открыть `backend/TicTacToe.sln` в Visual Studio 2022.

Выбрать профиль запуска **http** (или **https**) и нажать F5.

API поднимается на `http://localhost:5000`, Swagger открывается автоматически.

Миграции применяются при старте автоматически — ничего дополнительно делать не нужно.

**Из командной строки:**

```bash
cd backend
dotnet run --project src/TicTacToe.API
```

### 3. Frontend

```bash
cd tictactoe-ui
npm install
npm start
```

Приложение откроется на `http://localhost:4200`.  
Запросы к `/api/` автоматически проксируются на `http://localhost:5000`.

**Тесты:**

```bash
npm test
```

---

## Архитектура

```
docker-compose.yml
├── tictactoe-ui/          # Angular 21 (nginx:alpine в Docker)
│   ├── src/app/
│   │   ├── components/    # game, board, cell, login, leaderboard
│   │   ├── services/      # GameService, SessionService
│   │   ├── guards/        # authGuard
│   │   └── models/        # TypeScript типы
│   └── Dockerfile
└── backend/               # ASP.NET Core 8
    ├── TicTacToe.sln
    ├── src/
    │   ├── TicTacToe.Domain/        # сущности, перечисления
    │   ├── TicTacToe.Application/   # CQRS (MediatR), minimax AI
    │   ├── TicTacToe.Infrastructure/# EF Core, репозитории
    │   └── TicTacToe.API/           # контроллеры, Swagger
    └── src/TicTacToe.API/Dockerfile
```

**Зависимости между слоями:**

```
Domain ← Application ← Infrastructure
                      ← API
```

## Технологии

| Слой | Стек |
|---|---|
| Frontend | Angular 21, Standalone Components, Zoneless CD |
| Backend | ASP.NET Core 8, MediatR, FluentValidation, EF Core 8 |
| База данных | PostgreSQL 17 |
| Тесты (frontend) | Vitest |
| Логирование | Serilog |
| API документация | Swagger / Swashbuckle |
