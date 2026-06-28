import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);

  confirmEmail(email: string, token: string): Promise<void> {
    return firstValueFrom(this.http.post<void>('/api/auth/confirm-email', { email, token }));
  }
}
