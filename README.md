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
| Visual Studio | 2022+ |
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

### 2. Backend

Открыть `backend/TicTacToe.sln` в Visual Studio, выбрать профиль **http** и нажать F5.  
API поднимается на `http://localhost:5000`, Swagger открывается автоматически.  
Миграции применяются при старте — ничего дополнительного делать не нужно.

**Командная строка:**

```bash
cd backend
dotnet run --project src/TicTacToe.API
```

### 3. Frontend

Открыть корневой `TicTacToe.sln` в Visual Studio.  
В Solution Explorer правой кнопкой на `tictactoe-ui` → **Set as Startup Project**, затем F5.  
Angular dev-сервер запускается командой `npm start`, браузер открывается на `http://localhost:4200`.  
Фронтенд обращается к API напрямую по адресу `http://localhost:5000/api` (CORS настроен).

**Командная строка:**

```bash
cd tictactoe-ui
npm install
npm start
```

**Тесты:**

```bash
npm test
```

### 4. Полный стек из Visual Studio

Открыть корневой `TicTacToe.sln`. Настроить несколько стартовых проектов:

1. Правой кнопкой на Solution → **Properties**
2. Common Properties → **Startup Project** → **Multiple startup projects**
3. Для `TicTacToe.API` и `tictactoe-ui` выбрать Action = **Start**
4. Нажать F5 — запустятся оба проекта одновременно

---

## Архитектура

```
TicTacToe.sln              # корневое решение (фронтенд + бэкенд)
docker-compose.yml
├── tictactoe-ui/          # Angular 21 (nginx:alpine в Docker)
│   ├── tictactoe-ui.esproj
│   ├── src/app/
│   │   ├── components/    # game, board, cell, login, leaderboard
│   │   ├── services/      # GameService, SessionService
│   │   ├── guards/        # authGuard
│   │   └── models/        # TypeScript типы
│   └── Dockerfile
└── backend/               # ASP.NET Core 8
    ├── TicTacToe.sln      # решение только для бэкенда
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
