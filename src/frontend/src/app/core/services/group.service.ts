import { Injectable, inject } from '@angular/core';
import { Api } from '../../api/api';
import { CreateGroupRequest } from '../../api/models/create-group-request';
import { GroupDto } from '../../api/models/group-dto';
import { apiGroupsPost$Json, apiGroupsGet$Json, apiGroupsIdGet$Json } from '../../api/functions';

@Injectable({
  providedIn: 'root'
})
export class GroupService {
  private readonly api = inject(Api);

  async createGroup(req: CreateGroupRequest): Promise<string> {
    return await this.api.invoke(apiGroupsPost$Json, { body: req });
  }

  async getGroups(groupLeaderId?: string): Promise<GroupDto[]> {
    return await this.api.invoke(apiGroupsGet$Json, { groupLeaderId });
  }

  async getGroupById(id: string): Promise<GroupDto> {
    return await this.api.invoke(apiGroupsIdGet$Json, { id });
  }
}

