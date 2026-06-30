import { Injectable, inject } from '@angular/core';
import { Api } from '../../api/api';
import { PagedResponseOfEmailAuditLogDto } from '../../api/models/paged-response-of-email-audit-log-dto';
import { apiEmailAuditLogsGet$Json } from '../../api/functions';

@Injectable({
  providedIn: 'root'
})
export class EmailAuditLogService {
  private readonly api = inject(Api);

  async getEmailLogs(
    page: number,
    pageSize: number,
    filters?: { recipient?: string; success?: boolean }
  ): Promise<PagedResponseOfEmailAuditLogDto> {
    return await this.api.invoke(apiEmailAuditLogsGet$Json, {
      page,
      pageSize,
      recipient: filters?.recipient,
      success: filters?.success
    });
  }
}
