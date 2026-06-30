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
  template: `
    <div class="space-y-6 font-['Inter']">
      
      <!-- Top Action Header -->
      <div class="flex justify-between items-center bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md">
        <div>
          <h2 class="text-xl font-bold font-['Outfit'] text-white">Relatórios Automatizados (E-mails Agendados)</h2>
          <p class="text-xs text-[var(--color-text-secondary)] mt-1">Configure os horários de envio e destinatários dos resumos.</p>
        </div>
        <button
          (click)="showCreateForm.set(!showCreateForm())"
          class="px-4 py-2 bg-[var(--color-accent-brand)] hover:opacity-90 text-white rounded-lg text-xs font-semibold transition-all shadow-md shadow-indigo-500/20 cursor-pointer"
        >
          {{ showCreateForm() ? 'Fechar Formuário' : '+ Novo Agendamento' }}
        </button>
      </div>

      <!-- Create Form Card -->
      <div *ngIf="showCreateForm()" class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md space-y-4">
        <h3 class="text-sm font-semibold text-white uppercase tracking-wider">Criar Novo Agendamento</h3>
        
        <form [formGroup]="scheduleForm" (ngSubmit)="onCreateSchedule()" class="space-y-4">
          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label for="name" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Nome do Agendamento
              </label>
              <input
                id="name"
                type="text"
                formControlName="name"
                placeholder="Ex: Resumo Diário do Grupo"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-3 py-2 text-white placeholder-gray-500 text-sm focus-visible:outline-none transition-colors"
              />
              <div *ngIf="scheduleForm.get('name')?.touched && scheduleForm.get('name')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1 font-medium">
                O nome é obrigatório.
              </div>
            </div>

            <div>
              <label for="frequency" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Frequência
              </label>
              <select
                id="frequency"
                formControlName="frequency"
                (change)="onFrequencyChange()"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-3 py-2 text-white text-sm focus-visible:outline-none transition-colors"
              >
                <option *ngFor="let freq of frequencies" [value]="freq">
                  {{ FrequencyLabels[freq] }}
                </option>
              </select>
            </div>
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label for="cronExpression" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Expressão Cron
              </label>
              <input
                id="cronExpression"
                type="text"
                formControlName="cronExpression"
                placeholder="Ex: 0 8 * * *"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-3 py-2 text-white placeholder-gray-500 text-sm focus-visible:outline-none transition-colors"
              />
              <div *ngIf="scheduleForm.get('cronExpression')?.touched && scheduleForm.get('cronExpression')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1 font-medium">
                A expressão cron é obrigatória.
              </div>
            </div>

            <div *ngIf="scheduleForm.get('frequency')?.value === ReportFrequency.SpecificDay">
              <label for="specificDayOfMonth" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Dia Específico do Mês (1-31)
              </label>
              <input
                id="specificDayOfMonth"
                type="number"
                formControlName="specificDayOfMonth"
                min="1"
                max="31"
                placeholder="Ex: 5"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-3 py-2 text-white placeholder-gray-500 text-sm focus-visible:outline-none transition-colors"
              />
              <div *ngIf="scheduleForm.get('specificDayOfMonth')?.touched && scheduleForm.get('specificDayOfMonth')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1 font-medium">
                O dia do mês deve ser entre 1 e 31.
              </div>
            </div>
          </div>

          <div class="pt-2 flex gap-3">
            <button
              type="submit"
              [disabled]="scheduleForm.invalid"
              class="px-4 py-2.5 bg-[var(--color-accent-brand)] hover:opacity-90 disabled:opacity-50 text-white rounded-lg text-xs font-semibold transition-all cursor-pointer"
            >
              Criar Agendamento
            </button>
            <button
              type="button"
              (click)="showCreateForm.set(false)"
              class="px-4 py-2.5 bg-white/5 hover:bg-white/10 border border-white/10 text-white rounded-lg text-xs font-semibold transition-colors cursor-pointer"
            >
              Cancelar
            </button>
          </div>
        </form>
      </div>

      <!-- Schedules List -->
      <div class="space-y-4">
        <div *ngFor="let sched of schedules()" class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md space-y-4">
          <div class="flex justify-between items-start">
            <div>
              <h3 class="text-lg font-bold text-white">{{ sched.name }}</h3>
              <div class="flex items-center gap-2 mt-1">
                <span class="px-2 py-0.5 bg-indigo-500/10 text-indigo-300 border border-indigo-500/20 rounded text-xs font-semibold">
                  {{ FrequencyLabels[sched.frequency] }}
                </span>
                <span class="text-xs font-mono text-gray-400">
                  Cron: "{{ sched.cronExpression }}"
                </span>
                <span *ngIf="sched.specificDayOfMonth" class="text-xs text-gray-300 font-medium">
                  Dia: {{ sched.specificDayOfMonth }}
                </span>
              </div>
            </div>

            <button
              (click)="activeAddReceiverId.set(activeAddReceiverId() === sched.id ? null : sched.id)"
              class="px-3 py-1.5 bg-white/5 hover:bg-white/10 border border-white/10 text-white rounded text-xs font-semibold transition-colors cursor-pointer"
            >
              {{ activeAddReceiverId() === sched.id ? 'Fechar Destinatário' : '+ Adicionar Destinatário' }}
            </button>
          </div>

          <!-- Add Receiver Box -->
          <div *ngIf="activeAddReceiverId() === sched.id" class="p-4 bg-white/5 border border-white/5 rounded-lg space-y-3">
            <h4 class="text-xs font-bold text-[var(--color-text-secondary)] uppercase tracking-wider">Novo Destinatário</h4>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label class="block text-xs text-gray-300 mb-1.5">Usuário (Líder, Parceiro ou Gestor)</label>
                <select
                  [(ngModel)]="selectedUserId"
                  class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 rounded px-2.5 py-1.5 text-white text-xs focus-visible:outline-none"
                >
                  <option [value]="''">-- Selecionar Usuário --</option>
                  <option *ngFor="let usr of users()" [value]="usr.id">
                    {{ usr.displayName }} ({{ usr.email }} - {{ usr.role }})
                  </option>
                </select>
              </div>

              <div>
                <label class="block text-xs text-gray-300 mb-1.5">Contato Cliente (Opcional)</label>
                <select
                  [(ngModel)]="selectedClientContactId"
                  class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 rounded px-2.5 py-1.5 text-white text-xs focus-visible:outline-none"
                >
                  <option [value]="''">-- Nenhum Contato Cliente --</option>
                  <option *ngFor="let ct of clientContacts()" [value]="ct.id">
                    {{ ct.name }} ({{ ct.email }})
                  </option>
                </select>
              </div>
            </div>

            <div class="flex gap-2">
              <button
                (click)="onAddReceiver(sched.id)"
                [disabled]="!selectedUserId"
                class="px-3.5 py-1.5 bg-emerald-600 hover:opacity-90 disabled:opacity-50 text-white rounded text-xs font-semibold cursor-pointer"
              >
                Confirmar
              </button>
              <button
                (click)="activeAddReceiverId.set(null)"
                class="px-3.5 py-1.5 bg-white/5 hover:bg-white/10 text-gray-400 rounded text-xs font-semibold cursor-pointer"
              >
                Cancelar
              </button>
            </div>
          </div>

          <!-- Receivers List -->
          <div class="space-y-2">
            <h4 class="text-xs font-bold text-[var(--color-text-secondary)] uppercase tracking-wider">Destinatários Agendados:</h4>
            
            <div *ngIf="sched.receivers.length === 0" class="text-xs text-gray-500">
              Nenhum destinatário configurado para este agendamento.
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
              <div *ngFor="let rec of sched.receivers" class="p-3 bg-white/5 border border-white/5 rounded-lg flex items-center justify-between">
                <div>
                  <p class="text-sm font-semibold text-white">{{ rec.userDisplayName }}</p>
                  <p class="text-xs text-gray-400">{{ rec.userEmail }}</p>
                  <p *ngIf="rec.clientContactId" class="text-xs text-emerald-400 mt-1 font-medium">
                    Contato Cliente: {{ rec.clientContactName }} ({{ rec.clientContactEmail }})
                  </p>
                </div>
              </div>
            </div>
          </div>

        </div>

        <div *ngIf="schedules().length === 0" class="text-center py-12 bg-white/5 border border-white/10 rounded-xl text-gray-400 text-sm">
          Nenhum agendamento de e-mail cadastrado para este grupo.
        </div>
      </div>

    </div>
  `
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
