import { TestBed } from '@angular/core/testing';
import { DashboardComponent } from './dashboard.component';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { DailyActivityService } from '../../core/services/daily-activity.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('DashboardComponent', () => {
  let component: DashboardComponent;
  let mockAuthService: any;
  let mockDailyActivityService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockAuthService = {
      getRole: vi.fn().mockReturnValue('Partner'),
      logout: vi.fn()
    };
    mockDailyActivityService = {
      getActivities: vi.fn().mockResolvedValue([])
    };
    mockRouter = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [DashboardComponent],
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: DailyActivityService, useValue: mockDailyActivityService },
        { provide: Router, useValue: mockRouter }
      ]
    });

    const fixture = TestBed.createComponent(DashboardComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
