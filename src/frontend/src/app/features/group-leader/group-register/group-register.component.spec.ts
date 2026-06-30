import { TestBed } from '@angular/core/testing';
import { GroupRegisterComponent } from './group-register.component';
import { GroupService } from '../../../core/services/group.service';
import { CompanyService } from '../../../core/services/company.service';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('GroupRegisterComponent', () => {
  let component: GroupRegisterComponent;
  let mockGroupService: any;
  let mockCompanyService: any;
  let mockAuthService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockGroupService = {
      createGroup: vi.fn().mockResolvedValue({})
    };
    mockCompanyService = {
      getClientCompanies: vi.fn().mockResolvedValue({ items: [], totalItems: 0 })
    };
    mockAuthService = {
      getUserId: vi.fn().mockReturnValue('123')
    };
    mockRouter = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [GroupRegisterComponent],
      providers: [
        { provide: GroupService, useValue: mockGroupService },
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: { params: of({}) } },
        FormBuilder
      ]
    });

    const fixture = TestBed.createComponent(GroupRegisterComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
