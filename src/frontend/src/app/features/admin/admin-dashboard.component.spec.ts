import { TestBed } from '@angular/core/testing';
import { AdminDashboardComponent } from './admin-dashboard.component';
import { Router } from '@angular/router';
import { CompanyService } from '../../core/services/company.service';
import { UserService } from '../../core/services/user.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('AdminDashboardComponent', () => {
  let component: AdminDashboardComponent;
  let mockCompanyService: any;
  let mockUserService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockCompanyService = {
      getProviders: vi.fn().mockResolvedValue({ items: [], totalItems: 0 }),
      getClientCompanies: vi.fn().mockResolvedValue({ items: [], totalItems: 0 }),
      getProjects: vi.fn().mockResolvedValue({ items: [], totalItems: 0 })
    };
    mockUserService = {
      getUsers: vi.fn().mockResolvedValue([])
    };
    mockRouter = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [AdminDashboardComponent],
      providers: [
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: UserService, useValue: mockUserService },
        { provide: Router, useValue: mockRouter }
      ]
    });

    const fixture = TestBed.createComponent(AdminDashboardComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
