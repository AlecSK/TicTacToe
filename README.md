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

### 2. Backend + Frontend из Visual Studio (F5)

Открыть корневой `TicTacToe.sln` в Visual Studio. Убедиться что стартовый проект — **TicTacToe.API**, и нажать **F5**.

Что происходит автоматически:
1. ASP.NET Core поднимается на `http://localhost:5000`
2. SpaProxy запускает `npm start` — Angular стартует на `http://localhost:4200`
3. Браузер открывается и ждёт Angular (~30–60 секунд первая компиляция)
4. После готовности Angular браузер переходит на игру

Миграции применяются при старте — вручную ничего делать не нужно.

> **Первый запуск:** нужно ввести никнейм на странице входа — он создаётся в БД автоматически.  
> **Если видишь ошибку "Player not found"** — в браузере открой DevTools → Application → Local Storage → удали `ttt_nickname` и `ttt_game_id`, затем обнови страницу.

### 3. Только Frontend (отдельно)

```bash
cd tictactoe-ui
npm install
npm start
```

Angular поднимается на `http://localhost:4200`. Запросы к `/api` проксируются на `http://localhost:5000` — бэкенд должен быть запущен.

**Тесты:**

```bash
npm test
```

### 4. Backend из командной строки

```bash
cd backend
dotnet run --project src/TicTacToe.API
```

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
