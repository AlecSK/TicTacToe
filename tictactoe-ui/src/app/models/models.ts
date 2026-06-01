export type GameStatus = 'InProgress' | 'PlayerWon' | 'AiWon' | 'Draw';
export type CellMark = '-' | 'X' | 'O';

export interface PlayerDto {
  id: string;
  nickname: string;
  createdAt: string;
  lastSeen: string;
}

export interface PlayerStatsDto extends PlayerDto {
  totalGames: number;
  wins: number;
  losses: number;
  draws: number;
  winRate: number;
}

export interface GameStateDto {
  gameId: string;
  boardState: string;
  status: GameStatus;
  playerMark: CellMark;
  aiMark: CellMark;
  nextTurn: 'Player' | 'AI' | null;
}

export interface MoveResultDto extends GameStateDto {
  aiMove: number | null;
}

export interface PlayerGameDto {
  gameId: string;
  status: GameStatus;
  playerMark: CellMark;
  startedAt: string;
  finishedAt: string | null;
}

export interface LeaderboardEntryDto {
  id: string;
  nickname: string;
  totalGames: number;
  wins: number;
  losses: number;
  draws: number;
  winRate: number;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
}
