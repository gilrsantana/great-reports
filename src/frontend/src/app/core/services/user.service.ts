import { Injectable, inject } from '@angular/core';
import { Api } from '../../api/api';
import { apiUsersPost$Json, apiUsersGet$Json } from '../../api/functions';
import { RegisterUserRequest } from '../../api/models/register-user-request';
import { UserDto } from '../../api/models/user-dto';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly api = inject(Api);

  async registerUser(req: RegisterUserRequest): Promise<string> {
    return await this.api.invoke(apiUsersPost$Json, { body: req });
  }

  async getUsers(providerId: string): Promise<UserDto[]> {
    return await this.api.invoke(apiUsersGet$Json, { providerId });
  }
}
