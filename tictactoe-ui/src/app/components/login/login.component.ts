import { Component, inject, ChangeDetectorRef } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { PlayerService } from '../../services/player.service';
import { SessionService } from '../../services/session.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private readonly playerService = inject(PlayerService);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);
  private readonly cdr = inject(ChangeDetectorRef);

  nickname = '';
  loading = false;
  error = '';

  login(): void {
    const name = this.nickname.trim();
    if (!name) return;

    this.loading = true;
    this.error = '';

    this.playerService.login(name).subscribe({
      next: (player) => {
        this.session.setNickname(player.nickname);
        this.router.navigate(['/game']);
      },
      error: () => {
        this.error = 'Не удалось войти. Попробуй снова.';
        this.loading = false;
        this.cdr.markForCheck();
      }
    });
  }
}
