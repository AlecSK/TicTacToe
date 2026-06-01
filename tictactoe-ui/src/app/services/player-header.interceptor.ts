import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { SessionService } from './session.service';

export const playerHeaderInterceptor: HttpInterceptorFn = (req, next) => {
  const session = inject(SessionService);
  const nickname = session.getNickname();

  if (nickname) {
    const modified = req.clone({ setHeaders: { 'X-Player-Name': nickname } });
    return next(modified);
  }

  return next(req);
};
