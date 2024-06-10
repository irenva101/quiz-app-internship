import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { MsalService } from '@azure/msal-angular';

export const authGuard: CanActivateFn = (_route, _state) => {
  const authService = inject(MsalService);
  return authService.instance.getActiveAccount() != null;
};
