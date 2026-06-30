import { TestBed } from '@angular/core/testing';
import { CompanyService } from './company.service';
import { Api } from '../../api/api';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('CompanyService', () => {
  let service: CompanyService;
  let mockApi: any;

  beforeEach(() => {
    mockApi = {
      invoke: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        CompanyService,
        { provide: Api, useValue: mockApi }
      ]
    });

    service = TestBed.inject(CompanyService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call api for registerProvider', async () => {
    mockApi.invoke.mockResolvedValue('prov-guid');

    const result = await service.registerProvider({
      name: 'Test Provider',
      taxId: '12345678901234'
    });

    expect(result).toBe('prov-guid');
    expect(mockApi.invoke).toHaveBeenCalled();
  });

  it('should call api for getProviderDetails', async () => {
    mockApi.invoke.mockResolvedValue({
      id: 'prov-guid',
      name: 'Test Provider',
      taxId: '12345678901234'
    });

    const result = await service.getProviderDetails('prov-guid');

    expect(result.name).toBe('Test Provider');
    expect(result.id).toBe('prov-guid');
  });
});
