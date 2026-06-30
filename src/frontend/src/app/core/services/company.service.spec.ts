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

  it('should call api for registerClient', async () => {
    mockApi.invoke.mockResolvedValue('client-guid');

    const result = await service.registerClient({
      providerCompanyId: 'prov-guid',
      name: 'ACME Client'
    });

    expect(result).toBe('client-guid');
  });

  it('should call api for getClientCompanies and map paged response', async () => {
    mockApi.invoke.mockResolvedValue({
      items: [{ id: 'client-1', name: 'Client One' }],
      page: 1,
      pageSize: 5,
      totalCount: 1,
      totalPages: 1
    });

    const result = await service.getClientCompanies('prov-guid', 1, 5);

    expect(result.items.length).toBe(1);
    expect(result.items[0].id).toBe('client-1');
    expect(result.items[0].providerCompanyId).toBe('prov-guid');
    expect(result.totalCount).toBe(1);
  });
});
