import { TestBed } from '@angular/core/testing';
import { EmailConfirmationComponent } from './email-confirmation.component';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { of } from 'rxjs';

describe('EmailConfirmationComponent', () => {
  let component: EmailConfirmationComponent;
  let mockAuthService: any;
  let mockRouter: any;
  let mockActivatedRoute: any;

  beforeEach(() => {
    mockAuthService = {
      confirmEmail: vi.fn().mockResolvedValue(true)
    };
    mockRouter = {
      navigate: vi.fn()
    };
    mockActivatedRoute = {
      queryParams: of({ token: 'test-token', email: 'test@email.com' })
    };

    TestBed.configureTestingModule({
      imports: [EmailConfirmationComponent],
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: mockActivatedRoute }
      ]
    });

    const fixture = TestBed.createComponent(EmailConfirmationComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
