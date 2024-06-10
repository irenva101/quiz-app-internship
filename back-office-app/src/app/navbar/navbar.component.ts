import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { MsalService } from '@azure/msal-angular';
import { AuthenticationResult } from '@azure/msal-browser';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {
  constructor(
    private authService: MsalService,
    private router: Router
  ) {
    this.authService.handleRedirectObservable()
  }

  login() {
    this.authService.loginPopup()
      .subscribe((response: AuthenticationResult) => {
        this.authService.instance.setActiveAccount(response.account);
      });
  }

  logout() {
    this.authService.logoutRedirect({
      postLogoutRedirectUri: "/",
    });
    this.authService.instance.setActiveAccount(null);
  }

  navigateTo(route: string) {
    this.router.navigate([route]);
  }
  get loginDisplay(): boolean {
    return this.authService.instance.getActiveAccount() != null;
  }
}