import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { GameService } from '../../services/game.service';
import { SessionService } from '../../services/session.service';
import { LeaderboardEntryDto } from '../../models/models';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './leaderboard.component.html',
  styleUrl: './leaderboard.component.scss'
})
export class LeaderboardComponent implements OnInit {
  private readonly gameService = inject(GameService);
  private readonly session = inject(SessionService);

  currentNickname = this.session.getNickname() ?? '';
  entries: LeaderboardEntryDto[] = [];
  page = 1;
  pageSize = 10;
  totalCount = 0;
  loading = false;
  error = '';

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.gameService.getLeaderboard(this.page, this.pageSize).subscribe({
      next: (result) => {
        this.entries = result.items;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: () => {
        this.error = 'Не удалось загрузить лидерборд';
        this.loading = false;
      }
    });
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages) return;
    this.page = p;
    this.load();
  }
}
