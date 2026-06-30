import { TestBed } from '@angular/core/testing';
import { AuthService } from './auth.service';
import { Api } from '../../api/api';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('AuthService', () => {
  let service: AuthService;
  let mockApi: any;

  beforeEach(() => {
    mockApi = {
      invoke: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        AuthService,
        { provide: Api, useValue: mockApi }
      ]
    });

    service = TestBed.inject(AuthService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should set authenticated state on login', async () => {
    mockApi.invoke.mockResolvedValue({
      accessToken: 'a.b.c',
      refreshToken: 'ref'
    });

    const mockAtob = vi.spyOn(window, 'atob').mockReturnValue('{"email":"test@test.com","role":"Manager","exp":9999999999}');

    const response = await service.login('test@test.com', 'password123');

    expect(response.accessToken).toBe('a.b.c');
    expect(service.isAuthenticated()).toBe(true);
    expect(service.getRole()).toBe('Manager');

    mockAtob.mockRestore();
  });

  it('should clear session on logout', () => {
    service.currentUser.set({ email: 'test@test.com' });
    service.isAuthenticated.set(true);

    service.logout();

    expect(service.isAuthenticated()).toBe(false);
    expect(service.currentUser()).toBeNull();
  });
});
