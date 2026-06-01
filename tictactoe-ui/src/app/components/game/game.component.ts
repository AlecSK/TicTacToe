import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { BoardComponent } from '../board/board.component';
import { GameService } from '../../services/game.service';
import { SessionService } from '../../services/session.service';
import { GameStateDto } from '../../models/models';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, BoardComponent, RouterLink],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss'
})
export class GameComponent implements OnInit {
  private readonly gameService = inject(GameService);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  nickname = this.session.getNickname() ?? '';
  game: GameStateDto | null = null;
  loading = false;
  error = '';

  get isBoardDisabled(): boolean {
    return this.loading || !this.game || this.game.status !== 'InProgress' || this.game.nextTurn !== 'Player';
  }

  get statusText(): string {
    if (!this.game) return '';
    switch (this.game.status) {
      case 'PlayerWon': return '🎉 Ты победил!';
      case 'AiWon': return '🤖 AI победил';
      case 'Draw': return '🤝 Ничья';
      case 'InProgress': return this.game.nextTurn === 'Player' ? 'Твой ход' : 'AI думает...';
    }
  }

  get isFinished(): boolean {
    return !!this.game && this.game.status !== 'InProgress';
  }

  ngOnInit(): void {
    const savedId = this.session.getGameId();
    if (savedId) {
      this.loadGame(savedId);
    }
  }

  startGame(playerStarts: boolean): void {
    this.loading = true;
    this.error = '';

    this.gameService.startGame(this.nickname, playerStarts).subscribe({
      next: (game) => {
        this.game = game;
        this.session.setGameId(game.gameId);
        this.loading = false;
      },
      error: () => {
        this.error = 'Не удалось начать игру';
        this.loading = false;
      }
    });
  }

  onCellClick(cellIndex: number): void {
    if (!this.game || this.isBoardDisabled) return;

    this.loading = true;
    this.gameService.makeMove(this.game.gameId, this.nickname, cellIndex).subscribe({
      next: (result) => {
        this.game = result;
        if (result.status !== 'InProgress') {
          this.session.clearGameId();
        }
        this.loading = false;
      },
      error: () => {
        this.error = 'Ошибка хода';
        this.loading = false;
      }
    });
  }

  resign(): void {
    if (!this.game) return;

    this.loading = true;
    this.gameService.resign(this.game.gameId).subscribe({
      next: (result) => {
        this.game = result;
        this.session.clearGameId();
        this.loading = false;
      },
      error: () => {
        this.error = 'Ошибка при сдаче';
        this.loading = false;
      }
    });
  }

  newGame(): void {
    this.game = null;
    this.session.clearGameId();
    this.error = '';
  }

  private loadGame(gameId: string): void {
    this.loading = true;
    this.gameService.getGame(gameId).subscribe({
      next: (game) => {
        this.game = game;
        this.loading = false;
      },
      error: () => {
        this.session.clearGameId();
        this.loading = false;
      }
    });
  }
}
