import { TestBed } from '@angular/core/testing';
import { ScheduledEmailConfigComponent } from './scheduled-email-config.component';
import { ScheduledEmailService } from '../../../core/services/scheduled-email.service';
import { UserService } from '../../../core/services/user.service';
import { CompanyService } from '../../../core/services/company.service';
import { FormBuilder } from '@angular/forms';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('ScheduledEmailConfigComponent', () => {
  let component: ScheduledEmailConfigComponent;
  let mockScheduledEmailService: any;
  let mockUserService: any;
  let mockCompanyService: any;

  beforeEach(() => {
    mockScheduledEmailService = {
      getSchedules: vi.fn().mockResolvedValue([])
    };
    mockUserService = {
      getUsers: vi.fn().mockResolvedValue([])
    };
    mockCompanyService = {
      getClientContacts: vi.fn().mockResolvedValue({ items: [], totalItems: 0 })
    };

    TestBed.configureTestingModule({
      imports: [ScheduledEmailConfigComponent],
      providers: [
        { provide: ScheduledEmailService, useValue: mockScheduledEmailService },
        { provide: UserService, useValue: mockUserService },
        { provide: CompanyService, useValue: mockCompanyService },
        FormBuilder
      ]
    });

    const fixture = TestBed.createComponent(ScheduledEmailConfigComponent);
    component = fixture.componentInstance;
    component.groupId = '123';
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
