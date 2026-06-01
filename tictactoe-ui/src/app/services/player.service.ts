import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PlayerDto, PlayerStatsDto, PlayerGameDto, PagedResult } from '../models/models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PlayerService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/players`;

  login(nickname: string): Observable<PlayerDto> {
    return this.http.post<PlayerDto>(`${this.baseUrl}/login`, { nickname });
  }

  getStats(nickname: string): Observable<PlayerStatsDto> {
    return this.http.get<PlayerStatsDto>(`${this.baseUrl}/${nickname}`);
  }

  getGames(nickname: string): Observable<PagedResult<PlayerGameDto>> {
    return this.http.get<PagedResult<PlayerGameDto>>(`${this.baseUrl}/${nickname}/games`);
  }
}
