import { Injectable } from '@angular/core';

const NICKNAME_KEY = 'ttt_nickname';
const GAME_ID_KEY = 'ttt_game_id';

@Injectable({ providedIn: 'root' })
export class SessionService {
  getNickname(): string | null {
    return localStorage.getItem(NICKNAME_KEY);
  }

  setNickname(nickname: string): void {
    localStorage.setItem(NICKNAME_KEY, nickname);
  }

  getGameId(): string | null {
    return localStorage.getItem(GAME_ID_KEY);
  }

  setGameId(gameId: string): void {
    localStorage.setItem(GAME_ID_KEY, gameId);
  }

  clearGameId(): void {
    localStorage.removeItem(GAME_ID_KEY);
  }

  isLoggedIn(): boolean {
    return !!this.getNickname();
  }

  logout(): void {
    localStorage.removeItem(NICKNAME_KEY);
    localStorage.removeItem(GAME_ID_KEY);
  }
}
