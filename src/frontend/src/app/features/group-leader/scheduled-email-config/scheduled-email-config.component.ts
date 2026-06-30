import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ScheduledEmailService } from '../../../core/services/scheduled-email.service';
import { UserService } from '../../../core/services/user.service';
import { CompanyService } from '../../../core/services/company.service';
import { ScheduledEmailDto } from '../../../api/models/scheduled-email-dto';
import { ReportFrequency as ApiReportFrequency } from '../../../api/models/report-frequency';
import { UserDto } from '../../../api/models/user-dto';

export const ReportFrequency = {
  Daily: 0,
  Weekly: 1,
  TenDays: 2,
  TwelveDays: 3,
  FifteenDays: 4,
  Monthly: 5,
  SpecificDay: 6
} as const;

export const FrequencyLabels: Record<number, string> = {
  [ReportFrequency.Daily]: 'Diário',
  [ReportFrequency.Weekly]: 'Semanal',
  [ReportFrequency.TenDays]: 'A cada 10 dias',
  [ReportFrequency.TwelveDays]: 'A cada 12 dias',
  [ReportFrequency.FifteenDays]: 'A cada 15 dias',
  [ReportFrequency.Monthly]: 'Mensal',
  [ReportFrequency.SpecificDay]: 'Dia Específico do Mês'
};

@Component({
  selector: 'app-scheduled-email-config',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './scheduled-email-config.component.html',
  styleUrl: './scheduled-email-config.component.css'
})
export class ScheduledEmailConfigComponent implements OnInit {
  private readonly scheduledEmailService = inject(ScheduledEmailService);
  private readonly userService = inject(UserService);
  private readonly companyService = inject(CompanyService);
  private readonly fb = inject(FormBuilder);

  @Input() groupId!: string;
  @Input() clientCompanyId?: string;

  readonly FrequencyLabels = FrequencyLabels;
  readonly ReportFrequency = ReportFrequency;
  readonly frequencies = Object.values(ReportFrequency) as ApiReportFrequency[];

  readonly schedules = signal<ScheduledEmailDto[]>([]);
  readonly users = signal<UserDto[]>([]);
  readonly clientContacts = signal<any[]>([]);

  readonly showCreateForm = signal(false);
  readonly activeAddReceiverId = signal<string | null>(null);

  selectedUserId = '';
  selectedClientContactId = '';

  scheduleForm = this.fb.group({
    name: ['', [Validators.required]],
    frequency: [ReportFrequency.Daily as ApiReportFrequency, [Validators.required]],
    cronExpression: ['0 8 * * *', [Validators.required]],
    specificDayOfMonth: [null as number | null]
  });

  async ngOnInit() {
    await this.loadSchedules();
    await this.loadUsers();
    await this.loadClientContacts();
  }

  async loadSchedules() {
    try {
      const data = await this.scheduledEmailService.getGroupSchedules(this.groupId);
      this.schedules.set(data);
    } catch (e) {
      console.error(e);
    }
  }

  async loadUsers() {
    const providerId = localStorage.getItem('active_provider_id');
    if (!providerId) return;

    try {
      const usrs = await this.userService.getUsers(providerId);
      this.users.set(usrs);
    } catch (e) {
      console.error(e);
    }
  }

  async loadClientContacts() {
    if (!this.clientCompanyId) return;

    try {
      const cts = await this.companyService.getClientContacts(this.clientCompanyId);
      this.clientContacts.set(cts);
    } catch (e) {
      console.error(e);
    }
  }

  onFrequencyChange() {
    const freq = Number(this.scheduleForm.value.frequency) as ApiReportFrequency;
    let cron = '0 8 * * *';

    if (freq === ReportFrequency.Weekly) {
      cron = '0 8 * * 1'; // Monday 8 AM
    } else if (freq === ReportFrequency.Monthly) {
      cron = '0 8 1 * *'; // 1st of month 8 AM
    } else if (freq === ReportFrequency.SpecificDay) {
      const day = this.scheduleForm.value.specificDayOfMonth || 1;
      cron = `0 8 ${day} * *`;
    }

    this.scheduleForm.patchValue({ cronExpression: cron });

    const specDayCtrl = this.scheduleForm.get('specificDayOfMonth');
    if (freq === ReportFrequency.SpecificDay) {
      specDayCtrl?.setValidators([Validators.required, Validators.min(1), Validators.max(31)]);
    } else {
      specDayCtrl?.clearValidators();
    }
    specDayCtrl?.updateValueAndValidity();
  }

  async onCreateSchedule() {
    if (this.scheduleForm.invalid) return;

    const val = this.scheduleForm.value;

    try {
      await this.scheduledEmailService.createScheduledEmail({
        groupId: this.groupId,
        name: val.name!,
        frequency: Number(val.frequency!) as ApiReportFrequency,
        cronExpression: val.cronExpression!,
        specificDayOfMonth: Number(val.frequency!) === ReportFrequency.SpecificDay ? (val.specificDayOfMonth ?? null) : null
      });

      alert('Agendamento criado com sucesso!');
      this.showCreateForm.set(false);
      this.scheduleForm.reset({
        name: '',
        frequency: ReportFrequency.Daily as ApiReportFrequency,
        cronExpression: '0 8 * * *',
        specificDayOfMonth: null
      });
      await this.loadSchedules();
    } catch (e) {
      console.error(e);
      alert('Erro ao criar agendamento de e-mail.');
    }
  }

  async onAddReceiver(scheduleId: string) {
    if (!this.selectedUserId) return;

    try {
      await this.scheduledEmailService.addReceiver(scheduleId, {
        userId: this.selectedUserId,
        clientContactId: this.selectedClientContactId || null
      });

      alert('Destinatário adicionado com sucesso!');
      this.selectedUserId = '';
      this.selectedClientContactId = '';
      this.activeAddReceiverId.set(null);
      await this.loadSchedules();
    } catch (e) {
      console.error(e);
      alert('Erro ao adicionar destinatário.');
    }
  }
}
