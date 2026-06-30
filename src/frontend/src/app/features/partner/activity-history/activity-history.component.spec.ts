import { TestBed } from '@angular/core/testing';
import { ActivityHistoryComponent } from './activity-history.component';
import { DailyActivityService } from '../../../core/services/daily-activity.service';
import { Router } from '@angular/router';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ActivityHistoryComponent', () => {
  let component: ActivityHistoryComponent;
  let mockDailyActivityService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockDailyActivityService = {
      getActivities: vi.fn().mockResolvedValue([])
    };
    mockRouter = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [ActivityHistoryComponent],
      providers: [
        { provide: DailyActivityService, useValue: mockDailyActivityService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: { params: of({}) } }
      ]
    });

    const fixture = TestBed.createComponent(ActivityHistoryComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
