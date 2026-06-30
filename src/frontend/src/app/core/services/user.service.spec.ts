import { TestBed } from '@angular/core/testing';
import { UserService } from './user.service';
import { Api } from '../../api/api';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('UserService', () => {
  let service: UserService;
  let mockApi: any;

  beforeEach(() => {
    mockApi = {
      invoke: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        UserService,
        { provide: Api, useValue: mockApi }
      ]
    });

    service = TestBed.inject(UserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call api for registerUser', async () => {
    mockApi.invoke.mockResolvedValue('user-guid');

    const result = await service.registerUser({
      providerCompanyId: 'prov-guid',
      displayName: 'Name',
      email: 'user@test.com',
      role: 'Partner'
    });

    expect(result).toBe('user-guid');
    expect(mockApi.invoke).toHaveBeenCalled();
  });

  it('should call api for getUsers', async () => {
    mockApi.invoke.mockResolvedValue([
      { id: '1', displayName: 'User One', email: 'one@test.com', role: 'Partner', emailConfirmed: true }
    ]);

    const result = await service.getUsers('prov-guid');

    expect(result.length).toBe(1);
    expect(result[0].displayName).toBe('User One');
  });
});
