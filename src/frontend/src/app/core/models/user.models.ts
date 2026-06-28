export interface User {
  id: string;
  providerCompanyId: string;
  displayName: string;
  email: string;
  emailConfirmed: boolean;
}

export interface RegisterUserRequest {
  providerCompanyId: string;
  displayName: string;
  email: string;
  role: string;
}
