export interface ClientCompany {
  id: string;
  providerCompanyId: string;
  name: string;
}

export interface RegisterClientCompanyRequest {
  providerCompanyId: string;
  name: string;
}
