export interface Project {
  id: string;
  clientCompanyId: string;
  name: string;
  description: string;
}

export interface RegisterProjectRequest {
  clientCompanyId: string;
  name: string;
  description: string;
}
