import { Injectable, inject } from '@angular/core';
import { Api } from '../../api/api';
import { CreateDailyActivityRequest } from '../../api/models/create-daily-activity-request';
import { UpdateDailyActivityRequest } from '../../api/models/update-daily-activity-request';
import { DailyActivityDto } from '../../api/models/daily-activity-dto';
import { ActivityStatus } from '../../api/models/activity-status';
import {
  apiDailyActivitiesPost$Json,
  apiDailyActivitiesIdPut,
  apiDailyActivitiesGet$Json,
  apiDailyActivitiesLockoutStatusGet$Json,
  apiDailyActivitiesIdGet$Json
} from '../../api/functions';

@Injectable({
  providedIn: 'root'
})
export class DailyActivityService {
  private readonly api = inject(Api);

  async logActivity(req: CreateDailyActivityRequest): Promise<string> {
    return await this.api.invoke(apiDailyActivitiesPost$Json, { body: req });
  }

  async updateActivity(id: string, req: UpdateDailyActivityRequest): Promise<void> {
    return await this.api.invoke(apiDailyActivitiesIdPut, { id, body: req });
  }

  async getLockoutStatus(partnerId: string): Promise<boolean> {
    return await this.api.invoke(apiDailyActivitiesLockoutStatusGet$Json, { partnerId });
  }

  async getActivityById(id: string): Promise<DailyActivityDto> {
    return await this.api.invoke(apiDailyActivitiesIdGet$Json, { id });
  }

  async getActivities(filters: {
    partnerId: string;
    title?: string;
    theme?: string;
    status?: ActivityStatus;
    date?: string;
    page?: number;
    pageSize?: number;
  }): Promise<DailyActivityDto[]> {
    return await this.api.invoke(apiDailyActivitiesGet$Json, {
      partnerId: filters.partnerId,
      title: filters.title,
      theme: filters.theme,
      status: filters.status,
      date: filters.date,
      page: filters.page || 1,
      pageSize: filters.pageSize || 20
    });
  }
}
