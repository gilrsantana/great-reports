import { TestBed } from '@angular/core/testing';
import { DailyActivityService } from './daily-activity.service';
import { Api } from '../../api/api';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivityStatus } from '../../api/models/activity-status';

describe('DailyActivityService', () => {
  let service: DailyActivityService;
  let mockApi: any;

  beforeEach(() => {
    mockApi = {
      invoke: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        DailyActivityService,
        { provide: Api, useValue: mockApi }
      ]
    });

    service = TestBed.inject(DailyActivityService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should log activity via API', async () => {
    mockApi.invoke.mockResolvedValue('activity-guid');

    const req = {
      partnerId: 'partner-guid',
      title: 'Atividade 1',
      theme: 'Tema',
      content: 'Conteúdo',
      referenceDate: '2026-06-30',
      status: 0 as ActivityStatus,
      isBlocked: false
    };

    const res = await service.logActivity(req);

    expect(res).toBe('activity-guid');
    expect(mockApi.invoke).toHaveBeenCalled();
  });

  it('should check lockout status', async () => {
    mockApi.invoke.mockResolvedValue(true);

    const res = await service.getLockoutStatus('partner-guid');

    expect(res).toBe(true);
    expect(mockApi.invoke).toHaveBeenCalled();
  });
});
