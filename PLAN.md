# План: TicTacToe — Крестики-нолики

## Контекст

Полнофункциональное веб-приложение "Крестики-нолики" с тремя компонентами:
- Игрок vs AI (minimax — оптимальная игра AI)
- Авторизация только по никнейму (без пароля, без JWT)
- История партий, таблица лидеров и статистика в PostgreSQL
- Всё поднимается через docker-compose

---

## Структура репозитория

```
tictactoe/                          # c:\Repos\Claude\WorktreeTest\
├── backend/
│   ├── TicTacToe.sln
│   ├── Directory.Build.props       # Nullable enable, LangVersion 12
│   └── src/
│       ├── TicTacToe.Domain/
│       ├── TicTacToe.Application/
│       ├── TicTacToe.Infrastructure/
│       └── TicTacToe.API/
├── frontend/
│   └── tictactoe-ui/              # Angular standalone
├── docker-compose.yml
```

---

## Задача 1: Backend (C# ASP.NET Core 8)

### Проекты и их роли

| Проект | Роль | NuGet |
|--------|------|-------|
| `TicTacToe.Domain` | Сущности, интерфейсы репозиториев, исключения | — |
| `TicTacToe.Application` | CQRS (MediatR), бизнес-логика, FluentValidation | `MediatR`, `FluentValidation.DependencyInjectionExtensions` |
| `TicTacToe.Infrastructure` | EF Core, репозитории, GameEngine, MiniMax | `Npgsql.EntityFrameworkCore.PostgreSQL` |
| `TicTacToe.API` | Контроллеры, middleware, Program.cs | `Swashbuckle.AspNetCore`, `Serilog.AspNetCore` |

### Domain — ключевые сущности

```csharp
// Player: Id (Guid), Nickname (string), CreatedAt, LastSeen
// Game: Id, PlayerId, BoardState (CHAR 9, '-'=пусто), Status (enum), StartedAt, FinishedAt
// GameMove: Id, GameId, MoveNumber, CellIndex (0-8), IsAiMove, MadeAt
// GameStatus: InProgress=0, PlayerWon=1, AiWon=2, Draw=3
```

### Application — Commands / Queries

```
Players/
  Commands/LoginPlayer/   → создать или найти игрока по никнейму, обновить LastSeen
  Queries/GetPlayerStats/ → статистика (wins/losses/draws)
  Queries/GetLeaderboard/ → топ игроков с пагинацией

Games/
  Commands/StartGame/     → создать Game, если playerStarts=false — AI делает первый ход
  Commands/MakeMove/      → ход игрока → AI → проверка победителя → сохранить
  Queries/GetGame/        → текущее состояние партии
  Queries/GetPlayerGames/ → история партий игрока

Services/
  IGameEngine.cs          → CheckWinner, IsDraw, IsValidMove
  IMiniMaxService.cs      → GetBestMove(boardState, aiMark, playerMark) → cellIndex
```

### Infrastructure — ключевые реализации

**MiniMaxService** — полный minimax без альфа-бета (9 клеток, мгновенно):
```
GetBestMove → перебирает пустые клетки → MiniMax рекурсия → возвращает лучший cellIndex
Оценки: AI выиграл = +10, игрок выиграл = -10, ничья = 0
```

**Последовательность MakeMoveCommandHandler:**
1. Загрузить Game, проверить статус InProgress
2. Применить ход игрока (boardState[cellIndex] = 'X')
3. CheckWinner → PlayerWon? → сохранить и вернуть
4. IsDraw? → Draw, сохранить и вернуть
5. MiniMax → получить cellIndex AI → boardState[aiIndex] = 'O'
6. CheckWinner → AiWon? / IsDraw? → обновить статус
7. SaveChangesAsync → вернуть GameDto

### API Endpoints

| Метод | URL | Описание |
|-------|-----|----------|
| POST | `/api/players/login` | `{nickname}` → создать/найти игрока |
| GET | `/api/players/{nickname}` | профиль + статистика |
| GET | `/api/players/{nickname}/games` | история партий |
| POST | `/api/games` | `{nickname, playerStarts}` → новая партия |
| GET | `/api/games/{id}` | состояние партии |
| POST | `/api/games/{id}/moves` | `{nickname, cellIndex}` → ход + ответ AI |
| DELETE | `/api/games/{id}` | сдаться (→ AiWon) |
| GET | `/api/leaderboard` | `?page=1&pageSize=10` |

**Program.cs** — автоматическое применение миграций при старте:
```csharp
using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>().Database.MigrateAsync();
```

---

## Задача 2: Frontend (Angular, Standalone)

### Маршруты

```
/login        → LoginComponent     (ввод никнейма)
/game         → GameComponent      (доска + статус партии)   [authGuard]
/leaderboard  → LeaderboardComponent                          [authGuard]
```

authGuard: если никнейм не в localStorage → redirect `/login`

### Компоненты

| Компонент | Ответственность |
|-----------|----------------|
| `LoginComponent` | Форма никнейма → `PlayerService.login()` → localStorage → navigate `/game` |
| `GameComponent` | Управление партией: новая игра, ход, сдаться; показывает статус |
| `BoardComponent` | Сетка 3×3, Input: `boardState/disabled/playerMark`, Output: `cellClicked` |
| `CellComponent` | Одна клетка, CSS-классы `x-mark`/`o-mark`/`winning-cell` |
| `LeaderboardComponent` | Таблица лидеров с пагинацией, выделение текущего игрока |

### Сервисы и инфраструктура

- `SessionService` — обёртка над localStorage: nickname, gameId
- `PlayerService` — login, getStats, getGames
- `GameService` — startGame, getGame, makeMove, resign
- `PlayerHeaderInterceptor` — добавляет `X-Player-Name: {nickname}` к каждому запросу

### TypeScript типы

```typescript
type GameStatus = 'InProgress' | 'PlayerWon' | 'AiWon' | 'Draw';
type CellMark = '-' | 'X' | 'O';

interface GameStateDto {
  gameId: string; boardState: string; status: GameStatus;
  playerMark: CellMark; aiMark: CellMark; nextTurn: 'Player' | 'AI' | null;
}
interface MoveResultDto extends GameStateDto { aiMove: number | null; }
```

---

## Задача 3: База данных (PostgreSQL + EF Core)

### Таблицы

```sql
players:   id UUID PK, nickname VARCHAR(50) UNIQUE, created_at, last_seen
games:     id UUID PK, player_id UUID FK→players, board_state CHAR(9), status SMALLINT,
           player_starts BOOL, started_at, finished_at
game_moves: id BIGSERIAL PK, game_id UUID FK→games, move_number SMALLINT,
            cell_index SMALLINT, is_ai_move BOOL, made_at
            UNIQUE(game_id, move_number)
```

### View для лидерборда

```sql
CREATE VIEW leaderboard AS
SELECT p.id, p.nickname,
  COUNT(g.id) AS total_games,
  COUNT(g.id) FILTER (WHERE g.status = 1) AS wins,
  COUNT(g.id) FILTER (WHERE g.status = 2) AS losses,
  COUNT(g.id) FILTER (WHERE g.status = 3) AS draws,
  ROUND(COUNT(g.id) FILTER (WHERE g.status = 1)::numeric / NULLIF(COUNT(g.id),0)*100,1) AS win_rate
FROM players p LEFT JOIN games g ON g.player_id = p.id AND g.status != 0
GROUP BY p.id, p.nickname ORDER BY wins DESC;
```

View добавляется в миграцию через `migrationBuilder.Sql(...)`.

### Connection strings

- **Локально:** `Host=localhost;Port=5432;Database=tictactoe;Username=postgres;Password=postgres`
- **Docker** (env var перекрывает): `Host=postgres;Port=5432;Database=tictactoe;Username=postgres;Password=postgres`

---

## Docker Compose

```yaml
services:
  postgres:  image: postgres:17-alpine, port 5432, healthcheck pg_isready
  backend:   build ./backend, port 5000, depends_on postgres (healthy), env ConnectionStrings
  frontend:  build ./frontend/tictactoe-ui, port 4200:80, nginx proxy /api/ → backend:5000
```

**backend/Dockerfile** — multi-stage: `sdk:8.0` build → `aspnet:8.0` runtime  
**frontend/Dockerfile** — multi-stage: `node:22-alpine` build → `nginx:alpine` serve  
**nginx.conf** — SPA fallback (`try_files $uri /index.html`) + proxy `/api/` → backend

---

## Порядок реализации

Задачи можно реализовывать **параллельно** — они слабо связаны на этапе разработки:

| Задача | Зависимость | Параллельность |
|--------|-------------|----------------|
| **Задача 3 (БД)** | Нет — стартует первой | ✓ Независима |
| **Задача 1 (Backend)** | Domain entities (из задачи 3) | ✓ После Domain-слоя |
| **Задача 2 (Frontend)** | Знать форму API endpoints | ✓ По контракту из плана |
| **Docker** | Готовые Dockerfile-ы обоих | В конце |

**Рекомендуемая параллельная схема:**
- **Поток A:** Domain → EF Configurations → DbContext → миграция → репозитории → GameEngine/MiniMax → API контроллеры
- **Поток B (стартует одновременно):** TypeScript модели → SessionService → GameService/PlayerService → компоненты Angular
- **Финал:** Docker Compose когда оба потока завершены

---

## Верификация

### Docker

```powershell
docker-compose up --build -d
docker-compose logs -f backend   # "Now listening on: http://0.0.0.0:5000"
```

### curl (PowerShell)

```powershell
# Войти
curl -X POST http://localhost:5000/api/players/login -H "Content-Type: application/json" -d '{"nickname":"Vasya"}'

# Начать игру, записать gameId
curl -X POST http://localhost:5000/api/games -H "Content-Type: application/json" -d '{"nickname":"Vasya","playerStarts":true}'

# Сделать ход (центр = 4)
curl -X POST http://localhost:5000/api/games/{gameId}/moves -H "Content-Type: application/json" -d '{"nickname":"Vasya","cellIndex":4}'

# Лидерборд
curl http://localhost:5000/api/leaderboard

# Swagger
start http://localhost:5000/swagger
```

### Браузер

```
1. http://localhost:4200 → страница логина
2. Ввести никнейм → перейти на доску
3. Кликнуть клетку → AI отвечает
4. Несколько партий → /leaderboard → проверить статистику
5. F5 на /game → остаться на игре (authGuard работает)
```

### БД напрямую

```powershell
docker exec -it tictactoe-db psql -U postgres -d tictactoe -c "SELECT * FROM leaderboard LIMIT 10;"
docker exec -it tictactoe-db psql -U postgres -d tictactoe -c "SELECT * FROM games ORDER BY started_at DESC LIMIT 5;"
```

### Проверка AI

Minimax гарантирует оптимальную игру — игрок **никогда не победит**. Сыграть 3–4 партии пытаясь выиграть: все должны завершиться Draw или AiWon.
