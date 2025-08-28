import { HttpInterceptorFn } from '@angular/common/http';
import { delay, finalize, identity } from 'rxjs';

import { inject } from '@angular/core';
import { BusyService } from '../services/busy.service';
import { environment } from '../../../environments/environment.development';

export const loadingInterceptor: HttpInterceptorFn = (req, next) => {
  const busyService = inject(BusyService);

  busyService.busy();

  return next(req).pipe(
    (environment.production ? identity : delay(1000)),

    finalize(() => busyService.idle())
  )
};
