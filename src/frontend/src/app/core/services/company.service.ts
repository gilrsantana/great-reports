import { Injectable, inject } from '@angular/core';
import { Api } from '../../api/api';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';

import {
  apiProviderCompaniesPost$Json,
  apiProviderCompaniesIdGet$Json,
  apiClientCompaniesPost$Json,
  apiClientCompaniesGet$Json,
  apiClientCompaniesClientCompanyIdContactsPost$Json,
  apiProjectsPost$Json,
  apiClientCompaniesClientCompanyIdContactsGet$Json
} from '../../api/functions';
import { RegisterProviderCompanyRequest } from '../../api/models/register-provider-company-request';
import { ProviderDetailsDto } from '../../api/models/provider-details-dto';
import { RegisterClientCompanyRequest } from '../../api/models/register-client-company-request';
import { AddClientContactRequest } from '../../api/models/add-client-contact-request';
import { RegisterProjectRequest } from '../../api/models/register-project-request';
import { PagedResponse } from '../models/paged-response.models';
import { ClientCompany } from '../models/client-company.models';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private readonly api = inject(Api);

  async registerProvider(req: RegisterProviderCompanyRequest): Promise<string> {
    return await this.api.invoke(apiProviderCompaniesPost$Json, { body: req });
  }

  async getProviderDetails(id: string): Promise<ProviderDetailsDto> {
    return await this.api.invoke(apiProviderCompaniesIdGet$Json, { id });
  }

  async registerClient(req: RegisterClientCompanyRequest): Promise<string> {
    return await this.api.invoke(apiClientCompaniesPost$Json, { body: req });
  }

  async getClientCompanies(providerId: string, page: number, pageSize: number): Promise<PagedResponse<ClientCompany>> {
    const response = await this.api.invoke(apiClientCompaniesGet$Json, { providerId, page, pageSize });
    return {
      items: (response.items || []).map(item => ({
        id: item.id,
        name: item.name,
        providerCompanyId: providerId
      })),
      page: Number(response.page),
      pageSize: Number(response.pageSize),
      totalCount: Number(response.totalCount),
      totalPages: Number(response.totalPages || 0)
    };
  }

  async addClientContact(clientCompanyId: string, req: AddClientContactRequest): Promise<string> {
    return await this.api.invoke(apiClientCompaniesClientCompanyIdContactsPost$Json, {
      clientCompanyId,
      body: req
    });
  }

  async registerProject(req: RegisterProjectRequest): Promise<string> {
    return await this.api.invoke(apiProjectsPost$Json, { body: req });
  }

  async getClientContacts(clientCompanyId: string): Promise<any[]> {
    return await this.api.invoke(apiClientCompaniesClientCompanyIdContactsGet$Json, { clientCompanyId });
  }
}
