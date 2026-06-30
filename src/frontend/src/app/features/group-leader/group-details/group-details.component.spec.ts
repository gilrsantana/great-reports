import { TestBed } from '@angular/core/testing';
import { GroupDetailsComponent } from './group-details.component';
import { GroupService } from '../../../core/services/group.service';
import { ActivatedRoute } from '@angular/router';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { of } from 'rxjs';
import { ScheduledEmailService } from '../../../core/services/scheduled-email.service';
import { UserService } from '../../../core/services/user.service';
import { CompanyService } from '../../../core/services/company.service';
import { FormBuilder } from '@angular/forms';

describe('GroupDetailsComponent', () => {
  let component: GroupDetailsComponent;
  let mockGroupService: any;
  let mockActivatedRoute: any;
  let mockScheduledEmailService: any;
  let mockUserService: any;
  let mockCompanyService: any;

  beforeEach(() => {
    mockGroupService = {
      getGroupById: vi.fn().mockResolvedValue({ id: '123', name: 'Test Group', clientCompanyId: 'c-123', projectId: 'p-123', timezone: 'UTC' })
    };
    mockActivatedRoute = {
      paramMap: of({ get: (key: string) => '123' })
    };
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
      imports: [GroupDetailsComponent],
      providers: [
        { provide: GroupService, useValue: mockGroupService },
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: ScheduledEmailService, useValue: mockScheduledEmailService },
        { provide: UserService, useValue: mockUserService },
        { provide: CompanyService, useValue: mockCompanyService },
        FormBuilder
      ]
    });

    const fixture = TestBed.createComponent(GroupDetailsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
