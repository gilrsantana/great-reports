import { TestBed } from '@angular/core/testing';
import { ProjectRegisterComponent } from './project-register.component';
import { CompanyService } from '../../../core/services/company.service';
import { Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { of } from 'rxjs';

describe('ProjectRegisterComponent', () => {
  let component: ProjectRegisterComponent;
  let mockCompanyService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockCompanyService = {
      getClientCompanies: vi.fn().mockReturnValue(of({ items: [], totalItems: 0 })),
      registerProject: vi.fn().mockResolvedValue({})
    };
    mockRouter = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [ProjectRegisterComponent],
      providers: [
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: Router, useValue: mockRouter },
        FormBuilder
      ]
    });

    const fixture = TestBed.createComponent(ProjectRegisterComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
