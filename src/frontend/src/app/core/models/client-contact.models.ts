export interface ClientContact {
  id: string;
  clientCompanyId: string;
  name: string;
  email: string;
  emailConfirmed: boolean;
  type: 'Commercial' | 'Tech';
}

export interface AddClientContactRequest {
  name: string;
  email: string;
  contactType: string;
}
