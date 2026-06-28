import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { PagedResponse } from '../models/paged-response.models';
import { ProviderCompany, RegisterProviderCompanyRequest } from '../models/provider-company.models';
import { ClientCompany, RegisterClientCompanyRequest } from '../models/client-company.models';
import { AddClientContactRequest } from '../models/client-contact.models';
import { RegisterProjectRequest } from '../models/project.models';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private readonly http = inject(HttpClient);

  registerProvider(req: RegisterProviderCompanyRequest): Promise<string> {
    return firstValueFrom(this.http.post<string>('/api/providercompanies', req));
  }

  getProviderDetails(id: string): Promise<ProviderCompany> {
    return firstValueFrom(this.http.get<ProviderCompany>(`/api/providercompanies/${id}`));
  }

  registerClient(req: RegisterClientCompanyRequest): Promise<string> {
    return firstValueFrom(this.http.post<string>('/api/clientcompanies', req));
  }

  getClientCompanies(providerId: string, page: number, pageSize: number): Promise<PagedResponse<ClientCompany>> {
    return firstValueFrom(
      this.http.get<PagedResponse<ClientCompany>>(
        `/api/clientcompanies?providerId=${providerId}&page=${page}&pageSize=${pageSize}`
      )
    );
  }

  addClientContact(clientCompanyId: string, req: AddClientContactRequest): Promise<string> {
    return firstValueFrom(this.http.post<string>(`/api/clientcompanies/${clientCompanyId}/contacts`, req));
  }

  registerProject(req: RegisterProjectRequest): Promise<string> {
    return firstValueFrom(this.http.post<string>('/api/projects', req));
  }
}
