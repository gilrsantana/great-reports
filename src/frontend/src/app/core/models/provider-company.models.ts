export interface ProviderCompany {
  id: string;
  name: string;
  taxId: string;
}

export interface RegisterProviderCompanyRequest {
  name: string;
  taxId: string;
}
