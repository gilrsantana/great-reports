import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { RegisterUserRequest } from '../models/user.models';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly http = inject(HttpClient);

  registerUser(req: RegisterUserRequest): Promise<string> {
    return firstValueFrom(this.http.post<string>('/api/users', req));
  }
}
