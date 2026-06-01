import { TestBed, ComponentFixture } from '@angular/core/testing';
import { provideZonelessChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { LeaderboardComponent } from './leaderboard.component';
import { GameService } from '../../services/game.service';
import { SessionService } from '../../services/session.service';
import { LeaderboardEntryDto } from '../../models/models';

const ENTRIES: LeaderboardEntryDto[] = [
  { id: '1', nickname: 'Alice', totalGames: 10, wins: 7, losses: 2, draws: 1, winRate: 70 },
  { id: '2', nickname: 'TestUser', totalGames: 5, wins: 2, losses: 2, draws: 1, winRate: 40 },
  { id: '3', nickname: 'Bob', totalGames: 3, wins: 1, losses: 2, draws: 0, winRate: 33.3 },
];

describe('LeaderboardComponent', () => {
  let fixture: ComponentFixture<LeaderboardComponent>;
  let component: LeaderboardComponent;
  let gameService: { getLeaderboard: ReturnType<typeof vi.fn> };
  let session: { getNickname: ReturnType<typeof vi.fn> };

  beforeEach(async () => {
    gameService = { getLeaderboard: vi.fn().mockReturnValue(of(ENTRIES)) };
    session = { getNickname: vi.fn().mockReturnValue('TestUser') };

    await TestBed.configureTestingModule({
      imports: [LeaderboardComponent],
      providers: [
        provideZonelessChangeDetection(),
        provideRouter([]),
        { provide: GameService, useValue: gameService },
        { provide: SessionService, useValue: session },
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(LeaderboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
    await fixture.whenStable();
  });

  it('загружает записи при инициализации', () => {
    expect(gameService.getLeaderboard).toHaveBeenCalledWith(1, 10);
    expect(component.entries.length).toBe(3);
  });

  it('отображает 3 строки таблицы', async () => {
    const rows = fixture.nativeElement.querySelectorAll('.lb-table tbody tr');
    expect(rows.length).toBe(3);
  });

  it('выделяет текущего игрока (TestUser — строка 2)', async () => {
    const rows = fixture.nativeElement.querySelectorAll('.lb-table tbody tr');
    expect(rows[1].classList.contains('current-player')).toBe(true);
    expect(rows[0].classList.contains('current-player')).toBe(false);
    expect(rows[2].classList.contains('current-player')).toBe(false);
  });

  it('отображает имя и winRate в строках', async () => {
    const rows = fixture.nativeElement.querySelectorAll('.lb-table tbody tr');
    expect(rows[0].textContent).toContain('Alice');
    expect(rows[0].textContent).toContain('70%');
  });

  it('отображает — для winRate = null', async () => {
    const nullEntry: LeaderboardEntryDto[] = [
      { id: '99', nickname: 'New', totalGames: 0, wins: 0, losses: 0, draws: 0, winRate: null as any }
    ];
    gameService.getLeaderboard.mockReturnValue(of(nullEntry));
    component.load();
    fixture.detectChanges();
    await fixture.whenStable();
    const row = fixture.nativeElement.querySelector('.lb-table tbody tr');
    expect(row.textContent).toContain('—');
  });

  it('показывает сообщение при пустом списке', async () => {
    gameService.getLeaderboard.mockReturnValue(of([]));
    component.load();
    fixture.detectChanges();
    await fixture.whenStable();
    const empty = fixture.nativeElement.querySelector('.empty');
    expect(empty).toBeTruthy();
    expect(empty.textContent).toContain('Пока нет завершённых игр');
  });

  it('показывает ошибку при сбое API', async () => {
    gameService.getLeaderboard.mockReturnValue(throwError(() => new Error('network')));
    component.load();
    fixture.detectChanges();
    await fixture.whenStable();
    expect(component.error).toBe('Не удалось загрузить лидерборд');
    const errorEl = fixture.nativeElement.querySelector('.error');
    expect(errorEl).toBeTruthy();
  });
});
