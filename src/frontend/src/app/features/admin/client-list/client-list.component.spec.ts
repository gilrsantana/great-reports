import { TestBed } from '@angular/core/testing';
import { ClientListComponent } from './client-list.component';
import { CompanyService } from '../../../core/services/company.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ClientListComponent', () => {
  let component: ClientListComponent;
  let mockCompanyService: any;

  beforeEach(() => {
    mockCompanyService = {
      getClientCompanies: vi.fn().mockResolvedValue({ items: [], totalItems: 0 })
    };

    TestBed.configureTestingModule({
      imports: [ClientListComponent],
      providers: [
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: ActivatedRoute, useValue: { params: of({}) } }
      ]
    });

    const fixture = TestBed.createComponent(ClientListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
