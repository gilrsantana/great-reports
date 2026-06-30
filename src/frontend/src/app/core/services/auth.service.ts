import { Injectable, inject, signal } from '@angular/core';
import { Api } from '../../api/api';
import {
  apiAuthLoginPost$Json,
  apiAuthConfirmEmailPost,
  apiAuthChangePasswordPost,
  apiAuthForgetPasswordPost,
  apiAuthResetPasswordPost
} from '../../api/functions';
import { TokenResponse } from '../../api/models/token-response';

function decodeToken(token: string): any {
  try {
    const parts = token.split('.');
    if (parts.length !== 3) return null;
    const payload = atob(parts[1].replace(/-/g, '+').replace(/_/g, '/'));
    return JSON.parse(payload);
  } catch (e) {
    return null;
  }
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly api = inject(Api);

  readonly currentUser = signal<any | null>(null);
  readonly isAuthenticated = signal<boolean>(false);

  constructor() {
    this.loadSession();
  }

  private loadSession() {
    const token = localStorage.getItem('accessToken');
    if (token) {
      const decoded = decodeToken(token);
      if (decoded && !this.isTokenExpired(decoded)) {
        this.currentUser.set(decoded);
        this.isAuthenticated.set(true);
      } else {
        this.clearSession();
      }
    }
  }

  private isTokenExpired(decoded: any): boolean {
    if (!decoded.exp) return false;
    const expiry = decoded.exp * 1000;
    return Date.now() >= expiry;
  }

  async login(email: string, password: string): Promise<TokenResponse> {
    const response = await this.api.invoke(apiAuthLoginPost$Json, {
      body: { email, password }
    });
    
    localStorage.setItem('accessToken', response.accessToken);
    localStorage.setItem('refreshToken', response.refreshToken);

    const decoded = decodeToken(response.accessToken);
    this.currentUser.set(decoded);
    this.isAuthenticated.set(true);

    return response;
  }

  async confirmEmail(email: string, token: string): Promise<void> {
    await this.api.invoke(apiAuthConfirmEmailPost, {
      body: { email, token }
    });
  }

  async changePassword(currentPassword: string, newPassword: string): Promise<void> {
    await this.api.invoke(apiAuthChangePasswordPost, {
      body: { currentPassword, newPassword }
    });
  }

  async forgotPassword(email: string): Promise<void> {
    await this.api.invoke(apiAuthForgetPasswordPost, {
      body: { email }
    });
  }

  async resetPassword(email: string, token: string, newPassword: string): Promise<void> {
    await this.api.invoke(apiAuthResetPasswordPost, {
      body: { email, token, newPassword }
    });
  }

  logout(): void {
    this.clearSession();
  }

  private clearSession() {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.currentUser.set(null);
    this.isAuthenticated.set(false);
  }

  getAccessToken(): string | null {
    return localStorage.getItem('accessToken');
  }

  getRole(): string | null {
    const user = this.currentUser();
    if (!user) return null;
    return user.role || user['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || null;
  }

  getEmail(): string | null {
    const user = this.currentUser();
    if (!user) return null;
    return user.email || user['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] || null;
  }

  getUserId(): string | null {
    const user = this.currentUser();
    if (!user) return null;
    return user.sub || user['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || null;
  }
}
