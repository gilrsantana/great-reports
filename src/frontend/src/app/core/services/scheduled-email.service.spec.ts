import { TestBed } from '@angular/core/testing';
import { ScheduledEmailService } from './scheduled-email.service';
import { Api } from '../../api/api';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ReportFrequency } from '../../api/models/report-frequency';

describe('ScheduledEmailService', () => {
  let service: ScheduledEmailService;
  let mockApi: any;

  beforeEach(() => {
    mockApi = {
      invoke: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        ScheduledEmailService,
        { provide: Api, useValue: mockApi }
      ]
    });

    service = TestBed.inject(ScheduledEmailService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should create scheduled email configuration', async () => {
    mockApi.invoke.mockResolvedValue('schedule-guid');

    const req = {
      groupId: 'group-guid',
      name: 'Resumo Diário',
      cronExpression: '0 8 * * *',
      frequency: 0 as ReportFrequency,
      specificDayOfMonth: null
    };

    const res = await service.createScheduledEmail(req);

    expect(res).toBe('schedule-guid');
    expect(mockApi.invoke).toHaveBeenCalled();
  });

  it('should add receiver to scheduled email', async () => {
    mockApi.invoke.mockResolvedValue(null);

    await service.addReceiver('schedule-guid', {
      userId: 'user-guid',
      clientContactId: null
    });

    expect(mockApi.invoke).toHaveBeenCalled();
  });

  it('should get group scheduled emails', async () => {
    mockApi.invoke.mockResolvedValue([]);

    const res = await service.getGroupSchedules('group-guid');

    expect(res).toEqual([]);
    expect(mockApi.invoke).toHaveBeenCalled();
  });
});
