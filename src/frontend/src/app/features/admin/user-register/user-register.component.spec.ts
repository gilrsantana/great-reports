import { TestBed } from '@angular/core/testing';
import { UserRegisterComponent } from './user-register.component';
import { UserService } from '../../../core/services/user.service';
import { Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('UserRegisterComponent', () => {
  let component: UserRegisterComponent;
  let mockUserService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockUserService = {
      registerUser: vi.fn().mockResolvedValue({})
    };
    mockRouter = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [UserRegisterComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: { params: of({}) } },
        FormBuilder
      ]
    });

    const fixture = TestBed.createComponent(UserRegisterComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
