import { TestBed } from '@angular/core/testing';
import { AccountManagementComponent } from './account-management.component';
import { AuthService } from '../../core/services/auth.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('AccountManagementComponent', () => {
  let component: AccountManagementComponent;
  let mockAuthService: any;

  beforeEach(() => {
    mockAuthService = {
      getUserId: vi.fn().mockReturnValue('123'),
      getDisplayName: vi.fn().mockReturnValue('Test User'),
      getEmail: vi.fn().mockReturnValue('test@test.com'),
      getRole: vi.fn().mockReturnValue('Partner'),
      updatePassword: vi.fn().mockResolvedValue(true)
    };

    TestBed.configureTestingModule({
      imports: [AccountManagementComponent],
      providers: [
        { provide: AuthService, useValue: mockAuthService }
      ]
    });

    const fixture = TestBed.createComponent(AccountManagementComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
