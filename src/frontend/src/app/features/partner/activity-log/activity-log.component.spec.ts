import { TestBed } from '@angular/core/testing';
import { ActivityLogComponent } from './activity-log.component';
import { DailyActivityService } from '../../../core/services/daily-activity.service';
import { Router } from '@angular/router';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ActivityLogComponent', () => {
  let component: ActivityLogComponent;
  let mockDailyActivityService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockDailyActivityService = {
      getActivities: vi.fn().mockResolvedValue([]),
      logActivity: vi.fn().mockResolvedValue({})
    };
    mockRouter = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [ActivityLogComponent],
      providers: [
        { provide: DailyActivityService, useValue: mockDailyActivityService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: { params: of({}) } }
      ]
    });

    const fixture = TestBed.createComponent(ActivityLogComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
