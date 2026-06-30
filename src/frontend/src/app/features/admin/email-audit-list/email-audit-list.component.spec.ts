import { TestBed } from '@angular/core/testing';
import { EmailAuditListComponent } from './email-audit-list.component';
import { EmailAuditLogService } from '../../../core/services/email-audit-log.service';
import { FormBuilder } from '@angular/forms';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('EmailAuditListComponent', () => {
  let component: EmailAuditListComponent;
  let mockEmailAuditLogService: any;

  beforeEach(() => {
    mockEmailAuditLogService = {
      getEmailAuditLogs: vi.fn().mockResolvedValue({ items: [], totalItems: 0 })
    };

    TestBed.configureTestingModule({
      imports: [EmailAuditListComponent],
      providers: [
        { provide: EmailAuditLogService, useValue: mockEmailAuditLogService },
        FormBuilder
      ]
    });

    const fixture = TestBed.createComponent(EmailAuditListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
