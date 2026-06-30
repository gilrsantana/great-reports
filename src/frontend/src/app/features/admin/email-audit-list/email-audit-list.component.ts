import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { EmailAuditLogService } from '../../../core/services/email-audit-log.service';
import { EmailAuditLogDto } from '../../../api/models/email-audit-log-dto';
import { LogDetailsModalComponent } from '../log-details-modal/log-details-modal.component';

@Component({
  selector: 'app-email-audit-list',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, LogDetailsModalComponent],
  templateUrl: './email-audit-list.component.html',
  styleUrl: './email-audit-list.component.css'
})
export class EmailAuditListComponent implements OnInit {
  private readonly emailAuditLogService = inject(EmailAuditLogService);
  private readonly fb = inject(FormBuilder);

  readonly logs = signal<EmailAuditLogDto[]>([]);
  readonly loading = signal(false);
  readonly selectedLog = signal<EmailAuditLogDto | null>(null);

  readonly currentPage = signal(1);
  readonly pageSize = 10;
  readonly totalPages = signal(1);
  readonly totalItems = signal(0);

  filterForm = this.fb.group({
    recipient: [''],
    status: ['']
  });

  async ngOnInit() {
    await this.loadLogs();
  }

  async loadLogs() {
    this.loading.set(true);

    const val = this.filterForm.value;
    const recipient = val.recipient || undefined;
    let success: boolean | undefined = undefined;
    if (val.status === 'true') success = true;
    if (val.status === 'false') success = false;

    try {
      const response = await this.emailAuditLogService.getEmailLogs(this.currentPage(), this.pageSize, {
        recipient,
        success
      });

      this.logs.set(response.items || []);
      this.totalPages.set(Number(response.totalPages) || 1);
      this.totalItems.set(Number(response.totalCount) || 0);
    } catch (e) {
      console.error(e);
      alert('Erro ao carregar registros de auditoria de e-mails.');
    } finally {
      this.loading.set(false);
    }
  }

  async applyFilters() {
    this.currentPage.set(1);
    await this.loadLogs();
  }

  async clearFilters() {
    this.filterForm.reset({
      recipient: '',
      status: ''
    });
    this.currentPage.set(1);
    await this.loadLogs();
  }

  async prevPage() {
    if (this.currentPage() > 1) {
      this.currentPage.set(this.currentPage() - 1);
      await this.loadLogs();
    }
  }

  async nextPage() {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.set(this.currentPage() + 1);
      await this.loadLogs();
    }
  }
}
