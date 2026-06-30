import { TestBed } from '@angular/core/testing';
import { ProviderRegisterComponent } from './provider-register.component';
import { CompanyService } from '../../../core/services/company.service';
import { Router } from '@angular/router';
import { FormBuilder } from '@angular/forms';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ProviderRegisterComponent', () => {
  let component: ProviderRegisterComponent;
  let mockCompanyService: any;
  let mockRouter: any;

  beforeEach(() => {
    mockCompanyService = {
      registerProvider: vi.fn().mockResolvedValue({})
    };
    mockRouter = {
      navigate: vi.fn()
    };

    TestBed.configureTestingModule({
      imports: [ProviderRegisterComponent],
      providers: [
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: Router, useValue: mockRouter },
        { provide: ActivatedRoute, useValue: { params: of({}) } },
        FormBuilder
      ]
    });

    const fixture = TestBed.createComponent(ProviderRegisterComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
