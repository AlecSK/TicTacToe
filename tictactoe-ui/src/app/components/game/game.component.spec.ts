import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { GameComponent } from './game.component';
import { GameService } from '../../services/game.service';
import { SessionService } from '../../services/session.service';
import { GameStateDto, MoveResultDto } from '../../models/models';

// ─── Test fixtures ───────────────────────────────────────────────────────────

const PLAYER_FIRST: GameStateDto = {
  gameId: 'game-1', boardState: '---------', status: 'InProgress',
  playerMark: 'X', aiMark: 'O', nextTurn: 'Player',
};

const AI_FIRST: GameStateDto = {
  gameId: 'game-2', boardState: 'O--------', status: 'InProgress',
  playerMark: 'X', aiMark: 'O', nextTurn: 'Player',
};

const AFTER_MOVE: MoveResultDto = {
  gameId: 'game-1', boardState: 'O---X----', status: 'InProgress',
  playerMark: 'X', aiMark: 'O', nextTurn: 'Player', aiMove: 0,
};

const PLAYER_WON: MoveResultDto = {
  gameId: 'game-1', boardState: 'XXXOO----', status: 'PlayerWon',
  playerMark: 'X', aiMark: 'O', nextTurn: null, aiMove: null,
};

const AI_WON: GameStateDto = {
  gameId: 'game-1', boardState: '---------', status: 'AiWon',
  playerMark: 'X', aiMark: 'O', nextTurn: null,
};

const DRAW: MoveResultDto = {
  gameId: 'game-1', boardState: 'XOXOOXXXO', status: 'Draw',
  playerMark: 'X', aiMark: 'O', nextTurn: null, aiMove: 8,
};

// ─── Helpers ─────────────────────────────────────────────────────────────────

function makeGameService() {
  return {
    startGame: vi.fn(),
    getGame: vi.fn(),
    makeMove: vi.fn(),
    resign: vi.fn(),
    getLeaderboard: vi.fn(),
  } as unknown as GameService;
}

function makeSessionService(nickname = 'TestUser', gameId: string | null = null) {
  return {
    getNickname: vi.fn().mockReturnValue(nickname),
    getGameId: vi.fn().mockReturnValue(gameId),
    setGameId: vi.fn(),
    clearGameId: vi.fn(),
    isLoggedIn: vi.fn().mockReturnValue(true),
  } as unknown as SessionService;
}

// ─── Suite ───────────────────────────────────────────────────────────────────

describe('GameComponent', () => {
  let fixture: ComponentFixture<GameComponent>;
  let component: GameComponent;
  let gameService: ReturnType<typeof makeGameService>;
  let session: ReturnType<typeof makeSessionService>;

  async function setup(gameId: string | null = null) {
    gameService = makeGameService();
    session = makeSessionService('TestUser', gameId);

    await TestBed.configureTestingModule({
      imports: [GameComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        { provide: GameService, useValue: gameService },
        { provide: SessionService, useValue: session },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(GameComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    await fixture.whenStable();
  }

  describe('стартовый экран', () => {
    beforeEach(() => setup());

    it('показывает экран выбора хода, доски нет', async () => {
      const el: HTMLElement = fixture.nativeElement;
      expect(el.querySelector('.start-screen')).toBeTruthy();
      expect(el.querySelector('app-board')).toBeFalsy();
    });

    it('никнейм берётся из SessionService', () => {
      expect(component.nickname).toBe('TestUser');
    });
  });

  // ─── Игрок ходит первым ───────────────────────────────────────────────────

  describe('игрок ходит первым (playerStarts = true)', () => {
    beforeEach(async () => {
      await setup();
      (gameService.startGame as ReturnType<typeof vi.fn>).mockReturnValue(of(PLAYER_FIRST));
      component.startGame(true);
      fixture.detectChanges();
      await fixture.whenStable();
    });

    it('вызывает API с playerStarts=true', () => {
      expect(gameService.startGame).toHaveBeenCalledWith('TestUser', true);
    });

    it('доска пустая — все 9 клеток свободны', () => {
      expect(component.game?.boardState).toBe('---------');
    });

    it('игрок играет за X', () => {
      expect(component.game?.playerMark).toBe('X');
    });

    it('ход — Player', () => {
      expect(component.game?.nextTurn).toBe('Player');
    });

    it('доска активна', () => {
      expect(component.isBoardDisabled).toBe(false);
    });

    it('статус "Твой ход"', () => {
      expect(component.statusText).toBe('Твой ход');
    });

    it('сохраняет gameId в сессии', () => {
      expect(session.setGameId).toHaveBeenCalledWith('game-1');
    });

    it('отображает доску в шаблоне', () => {
      expect(fixture.nativeElement.querySelector('app-board')).toBeTruthy();
    });
  });

  // ─── AI ходит первым ──────────────────────────────────────────────────────

  describe('AI ходит первым (playerStarts = false)', () => {
    beforeEach(async () => {
      await setup();
      (gameService.startGame as ReturnType<typeof vi.fn>).mockReturnValue(of(AI_FIRST));
      component.startGame(false);
      fixture.detectChanges();
      await fixture.whenStable();
    });

    it('вызывает API с playerStarts=false', () => {
      expect(gameService.startGame).toHaveBeenCalledWith('TestUser', false);
    });

    it('AI уже сделал ход — O на позиции 0', () => {
      expect(component.game?.boardState[0]).toBe('O');
    });

    it('оставшиеся 8 клеток пусты', () => {
      const free = component.game!.boardState.split('').filter(c => c === '-').length;
      expect(free).toBe(8);
    });

    it('игрок за X', () => {
      expect(component.game?.playerMark).toBe('X');
    });

    it('теперь ход игрока — доска активна', () => {
      expect(component.isBoardDisabled).toBe(false);
    });
  });

  // ─── Ход игрока ───────────────────────────────────────────────────────────

  describe('onCellClick', () => {
    beforeEach(async () => {
      await setup();
      (gameService.startGame as ReturnType<typeof vi.fn>).mockReturnValue(of(PLAYER_FIRST));
      component.startGame(true);
      await fixture.whenStable();
    });

    it('вызывает makeMove с нужными аргументами', () => {
      (gameService.makeMove as ReturnType<typeof vi.fn>).mockReturnValue(of(AFTER_MOVE));
      component.onCellClick(4);
      expect(gameService.makeMove).toHaveBeenCalledWith('game-1', 'TestUser', 4);
    });

    it('обновляет boardState после хода', async () => {
      (gameService.makeMove as ReturnType<typeof vi.fn>).mockReturnValue(of(AFTER_MOVE));
      component.onCellClick(4);
      fixture.detectChanges();
      await fixture.whenStable();
      expect(component.game?.boardState).toBe('O---X----');
    });

    it('AI ответил — O на позиции 0', async () => {
      (gameService.makeMove as ReturnType<typeof vi.fn>).mockReturnValue(of(AFTER_MOVE));
      component.onCellClick(4);
      await fixture.whenStable();
      expect(component.game?.boardState[0]).toBe('O');
    });

    it('при ошибке показывает сообщение', async () => {
      (gameService.makeMove as ReturnType<typeof vi.fn>).mockReturnValue(
        throwError(() => new Error('server error'))
      );
      component.onCellClick(4);
      fixture.detectChanges();
      await fixture.whenStable();
      expect(component.error).toBe('Ошибка хода');
    });

    it('не ходит если loading=true', () => {
      component.loading = true;
      component.onCellClick(0);
      expect(gameService.makeMove).not.toHaveBeenCalled();
    });
  });

  // ─── Исходы партии ────────────────────────────────────────────────────────

  describe('окончание игры', () => {
    beforeEach(async () => {
      await setup();
      (gameService.startGame as ReturnType<typeof vi.fn>).mockReturnValue(of(PLAYER_FIRST));
      component.startGame(true);
      await fixture.whenStable();
    });

    it('победа игрока — статус PlayerWon, очищает gameId', async () => {
      (gameService.makeMove as ReturnType<typeof vi.fn>).mockReturnValue(of(PLAYER_WON));
      component.onCellClick(2);
      fixture.detectChanges();
      await fixture.whenStable();
      expect(component.game?.status).toBe('PlayerWon');
      expect(component.statusText).toBe('🎉 Ты победил!');
      expect(session.clearGameId).toHaveBeenCalled();
    });

    it('resign — статус AiWon', async () => {
      (gameService.resign as ReturnType<typeof vi.fn>).mockReturnValue(of(AI_WON));
      component.resign();
      fixture.detectChanges();
      await fixture.whenStable();
      expect(component.game?.status).toBe('AiWon');
      expect(component.statusText).toBe('🤖 AI победил');
    });

    it('resign передаёт nickname в сервис', () => {
      (gameService.resign as ReturnType<typeof vi.fn>).mockReturnValue(of(AI_WON));
      component.resign();
      expect(gameService.resign).toHaveBeenCalledWith('game-1', 'TestUser');
    });

    it('ничья — статус Draw', async () => {
      (gameService.makeMove as ReturnType<typeof vi.fn>).mockReturnValue(of(DRAW));
      component.onCellClick(8);
      fixture.detectChanges();
      await fixture.whenStable();
      expect(component.game?.status).toBe('Draw');
      expect(component.statusText).toBe('🤝 Ничья');
    });

    it('isFinished = true при завершённой игре', async () => {
      (gameService.makeMove as ReturnType<typeof vi.fn>).mockReturnValue(of(PLAYER_WON));
      component.onCellClick(2);
      await fixture.whenStable();
      expect(component.isFinished).toBe(true);
    });

    it('isFinished = false во время игры', () => {
      expect(component.isFinished).toBe(false);
    });
  });

  // ─── Новая игра ───────────────────────────────────────────────────────────

  describe('newGame', () => {
    it('сбрасывает состояние и очищает gameId', async () => {
      await setup();
      (gameService.startGame as ReturnType<typeof vi.fn>).mockReturnValue(of(PLAYER_FIRST));
      component.startGame(true);
      component.error = 'какая-то ошибка';
      await fixture.whenStable();

      component.newGame();
      fixture.detectChanges();
      await fixture.whenStable();

      expect(component.game).toBeNull();
      expect(component.error).toBe('');
      expect(session.clearGameId).toHaveBeenCalled();
    });

    it('после newGame снова стартовый экран', async () => {
      await setup();
      (gameService.startGame as ReturnType<typeof vi.fn>).mockReturnValue(of(PLAYER_FIRST));
      component.startGame(true);
      fixture.detectChanges();
      await fixture.whenStable();

      component.newGame();
      fixture.detectChanges();
      await fixture.whenStable();

      expect(fixture.nativeElement.querySelector('.start-screen')).toBeTruthy();
    });
  });

  // ─── Восстановление сессии ────────────────────────────────────────────────

  describe('восстановление партии из localStorage', () => {
    async function setupWithSavedGame(gameId: string, getGameResult: any) {
      const gs = makeGameService();
      const ss = makeSessionService('TestUser', gameId);
      (gs.getGame as ReturnType<typeof vi.fn>).mockReturnValue(getGameResult);

      await TestBed.configureTestingModule({
        imports: [GameComponent],
        providers: [
          provideZonelessChangeDetection(),
          provideRouter([]),
          { provide: GameService, useValue: gs },
          { provide: SessionService, useValue: ss },
        ]
      }).compileComponents();

      const fix = TestBed.createComponent(GameComponent);
      fix.detectChanges();
      await fix.whenStable();
      return { fix, comp: fix.componentInstance, gs, ss };
    }

    it('при наличии gameId загружает партию', async () => {
      const { comp, gs } = await setupWithSavedGame('saved-game-id', of(PLAYER_FIRST));
      expect(gs.getGame).toHaveBeenCalledWith('saved-game-id');
      expect(comp.game).toEqual(PLAYER_FIRST);
    });

    it('при ошибке загрузки очищает gameId', async () => {
      const { comp, ss } = await setupWithSavedGame(
        'stale-id',
        throwError(() => new Error('not found')) as any
      );
      expect(ss.clearGameId).toHaveBeenCalled();
      expect(comp.game).toBeNull();
    });
  });

  // ─── Ошибка старта ────────────────────────────────────────────────────────

  it('показывает ошибку если startGame не удался', async () => {
    await setup();
    (gameService.startGame as ReturnType<typeof vi.fn>).mockReturnValue(
      throwError(() => new Error('network'))
    );
    component.startGame(true);
    fixture.detectChanges();
    await fixture.whenStable();
    expect(component.error).toBe('Не удалось начать игру');
    expect(component.loading).toBe(false);
  });
});
