import { TestBed } from '@angular/core/testing';
import { SessionService } from './session.service';

describe('SessionService', () => {
  let service: SessionService;

  beforeEach(() => {
    localStorage.clear();
    TestBed.configureTestingModule({});
    service = TestBed.inject(SessionService);
  });

  describe('nickname', () => {
    it('getNickname returns null initially', () => {
      expect(service.getNickname()).toBeNull();
    });

    it('setNickname + getNickname round-trips', () => {
      service.setNickname('Oleg');
      expect(service.getNickname()).toBe('Oleg');
    });

    it('isLoggedIn is false when no nickname', () => {
      expect(service.isLoggedIn()).toBe(false);
    });

    it('isLoggedIn is true after setNickname', () => {
      service.setNickname('Oleg');
      expect(service.isLoggedIn()).toBe(true);
    });
  });

  describe('gameId', () => {
    it('getGameId returns null initially', () => {
      expect(service.getGameId()).toBeNull();
    });

    it('setGameId + getGameId round-trips', () => {
      service.setGameId('abc-123');
      expect(service.getGameId()).toBe('abc-123');
    });

    it('clearGameId removes stored id', () => {
      service.setGameId('abc-123');
      service.clearGameId();
      expect(service.getGameId()).toBeNull();
    });
  });

  describe('logout', () => {
    it('clears nickname and gameId', () => {
      service.setNickname('Oleg');
      service.setGameId('abc-123');
      service.logout();
      expect(service.getNickname()).toBeNull();
      expect(service.getGameId()).toBeNull();
      expect(service.isLoggedIn()).toBe(false);
    });
  });
});
