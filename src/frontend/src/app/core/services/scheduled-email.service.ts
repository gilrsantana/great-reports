import { Injectable, inject } from '@angular/core';
import { Api } from '../../api/api';
import { CreateScheduledEmailRequest } from '../../api/models/create-scheduled-email-request';
import { AddScheduledEmailReceiverRequest } from '../../api/models/add-scheduled-email-receiver-request';
import { ScheduledEmailDto } from '../../api/models/scheduled-email-dto';
import {
  apiScheduledEmailsPost$Json,
  apiScheduledEmailsIdReceiversPost,
  apiScheduledEmailsGroupGroupIdGet$Json
} from '../../api/functions';

@Injectable({
  providedIn: 'root'
})
export class ScheduledEmailService {
  private readonly api = inject(Api);

  async createScheduledEmail(req: CreateScheduledEmailRequest): Promise<string> {
    return await this.api.invoke(apiScheduledEmailsPost$Json, { body: req });
  }

  async addReceiver(emailId: string, req: AddScheduledEmailReceiverRequest): Promise<void> {
    return await this.api.invoke(apiScheduledEmailsIdReceiversPost, { id: emailId, body: req });
  }

  async getGroupSchedules(groupId: string): Promise<ScheduledEmailDto[]> {
    return await this.api.invoke(apiScheduledEmailsGroupGroupIdGet$Json, { groupId });
  }
}
