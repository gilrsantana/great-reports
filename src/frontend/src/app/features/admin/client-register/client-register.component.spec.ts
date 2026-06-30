import { TestBed } from '@angular/core/testing';
import { ClientRegisterComponent } from './client-register.component';
import { CompanyService } from '../../../core/services/company.service';
import { Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ClientRegisterComponent', () => {
  let component: ClientRegisterComponent;
  let mockCompanyService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockCompanyService = {
      registerClientCompany: vi.fn().mockResolvedValue({})
    };
    mockRouter = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [ClientRegisterComponent],
      providers: [
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: { params: of({}) } },
        FormBuilder
      ]
    });

    const fixture = TestBed.createComponent(ClientRegisterComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
