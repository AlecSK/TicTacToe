import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { GameStateDto, MoveResultDto, LeaderboardEntryDto, PagedResult } from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class GameService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = environment.apiUrl;

  startGame(nickname: string, playerStarts: boolean): Observable<GameStateDto> {
    return this.http.post<GameStateDto>(`${this.apiUrl}/games`, { nickname, playerStarts });
  }

  getGame(gameId: string): Observable<GameStateDto> {
    return this.http.get<GameStateDto>(`${this.apiUrl}/games/${gameId}`);
  }

  makeMove(gameId: string, nickname: string, cellIndex: number): Observable<MoveResultDto> {
    return this.http.post<MoveResultDto>(`${this.apiUrl}/games/${gameId}/moves`, { nickname, cellIndex });
  }

  resign(gameId: string): Observable<GameStateDto> {
    return this.http.delete<GameStateDto>(`${this.apiUrl}/games/${gameId}`);
  }

  getLeaderboard(page: number = 1, pageSize: number = 10): Observable<PagedResult<LeaderboardEntryDto>> {
    return this.http.get<PagedResult<LeaderboardEntryDto>>(
      `${this.apiUrl}/leaderboard?page=${page}&pageSize=${pageSize}`
    );
  }
}
