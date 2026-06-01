import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
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
  private readonly cdr = inject(ChangeDetectorRef);

  currentNickname = this.session.getNickname() ?? '';
  entries: LeaderboardEntryDto[] = [];
  page = 1;
  pageSize = 10;
  loading = false;
  error = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.cdr.markForCheck();

    this.gameService.getLeaderboard(this.page, this.pageSize).subscribe({
      next: (items) => {
        this.entries = items;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: () => {
        this.error = 'Не удалось загрузить лидерборд';
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }
}
