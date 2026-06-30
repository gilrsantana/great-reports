import { TestBed } from '@angular/core/testing';
import { EmailAuditLogService } from './email-audit-log.service';
import { Api } from '../../api/api';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('EmailAuditLogService', () => {
  let service: EmailAuditLogService;
  let mockApi: any;

  beforeEach(() => {
    mockApi = {
      invoke: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        EmailAuditLogService,
        { provide: Api, useValue: mockApi }
      ]
    });

    service = TestBed.inject(EmailAuditLogService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get email logs with parameters', async () => {
    mockApi.invoke.mockResolvedValue({
      items: [],
      page: 1,
      pageSize: 10,
      totalCount: 0,
      totalPages: 1
    });

    const res = await service.getEmailLogs(1, 10, {
      recipient: 'test@test.com',
      success: true
    });

    expect(res.items).toEqual([]);
    expect(mockApi.invoke).toHaveBeenCalled();
  });
});
