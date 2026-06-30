import { TestBed } from '@angular/core/testing';
import { ProviderDetailsComponent } from './provider-details.component';
import { CompanyService } from '../../../core/services/company.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ProviderDetailsComponent', () => {
  let component: ProviderDetailsComponent;
  let mockCompanyService: any;

  beforeEach(() => {
    mockCompanyService = {
      getProviderDetails: vi.fn().mockResolvedValue({ id: '123', name: 'Test Provider', taxId: '123456' })
    };

    TestBed.configureTestingModule({
      imports: [ProviderDetailsComponent],
      providers: [
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: ActivatedRoute, useValue: { params: of({}) } }
      ]
    });

    const fixture = TestBed.createComponent(ProviderDetailsComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
