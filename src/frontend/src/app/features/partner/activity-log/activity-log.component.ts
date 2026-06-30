import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, ActivatedRoute, RouterModule } from '@angular/router';
import { DailyActivityService } from '../../../core/services/daily-activity.service';
import { AuthService } from '../../../core/services/auth.service';
import { ActivityStatus as ApiActivityStatus } from '../../../api/models/activity-status';
import { CreateDailyActivityRequest } from '../../../api/models/create-daily-activity-request';
import { UpdateDailyActivityRequest } from '../../../api/models/update-daily-activity-request';

export const ActivityStatus = {
  Doing: 0,
  Done: 1
} as const;

@Component({
  selector: 'app-activity-log',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="p-6 min-h-screen bg-[var(--color-bg-primary)] text-white px-4 font-['Inter']">
      <div class="max-w-2xl mx-auto space-y-6">
        
        <!-- Header Panel -->
        <div class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md flex justify-between items-center">
          <div>
            <h1 class="text-3xl font-extrabold tracking-tight font-['Outfit'] text-white">
              {{ isEditMode ? 'Editar Atividade' : 'Registrar Atividade' }}
            </h1>
            <p class="text-xs text-[var(--color-text-secondary)] mt-2 uppercase tracking-wider">
              Registre suas tarefas diárias e andamento do trabalho.
            </p>
          </div>
          <button routerLink="/parceiro/atividades" class="px-3.5 py-1.5 bg-white/5 hover:bg-white/10 border border-white/10 text-gray-300 rounded-lg text-sm transition-colors cursor-pointer">
            Voltar ao Histórico
          </button>
        </div>

        <!-- Lockout Warning Banner -->
        <div *ngIf="isLockedOut" class="p-4 bg-amber-500/10 border border-amber-500/30 text-amber-200 rounded-xl flex items-center gap-3 animate-pulse">
          <svg class="w-5 h-5 text-amber-400 shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
          </svg>
          <div>
            <h3 class="font-semibold text-sm">Bloqueio Ativo (Após 23:50)</h3>
            <p class="text-xs text-amber-300/80">O período de registro de atividades encerrou para hoje. As alterações estão desabilitadas.</p>
          </div>
        </div>

        <!-- Form Card -->
        <div class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md">
          <form [formGroup]="activityForm" (ngSubmit)="onSubmit()" class="space-y-5">
            
            <div>
              <label for="title" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Título da Atividade
              </label>
              <input
                id="title"
                type="text"
                formControlName="title"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white placeholder-gray-500 text-sm focus-visible:ring-2 focus-visible:ring-[var(--color-accent-brand)] focus-visible:outline-none transition-colors"
                placeholder="Ex: Refatoração do módulo de login"
              />
              <div *ngIf="activityForm.get('title')?.touched && activityForm.get('title')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
                O título é obrigatório.
              </div>
            </div>

            <div>
              <label for="theme" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Tema / Categoria
              </label>
              <input
                id="theme"
                type="text"
                formControlName="theme"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white placeholder-gray-500 text-sm focus-visible:ring-2 focus-visible:ring-[var(--color-accent-brand)] focus-visible:outline-none transition-colors"
                placeholder="Ex: Desenvolvimento, Bugs, DevOps"
              />
              <div *ngIf="activityForm.get('theme')?.touched && activityForm.get('theme')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
                O tema é obrigatório.
              </div>
            </div>

            <div>
              <label for="content" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Descrição Detalhada
              </label>
              <textarea
                id="content"
                rows="4"
                formControlName="content"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white placeholder-gray-500 text-sm focus-visible:ring-2 focus-visible:ring-[var(--color-accent-brand)] focus-visible:outline-none transition-colors resize-none"
                placeholder="Descreva detalhadamente o que foi feito..."
              ></textarea>
              <div *ngIf="activityForm.get('content')?.touched && activityForm.get('content')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
                A descrição detalhada é obrigatória.
              </div>
            </div>

            <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label for="referenceDate" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                  Data de Referência
                </label>
                <input
                  id="referenceDate"
                  type="date"
                  formControlName="referenceDate"
                  class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white text-sm focus-visible:outline-none transition-colors"
                />
              </div>

              <div>
                <label for="status" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                  Status
                </label>
                <select
                  id="status"
                  formControlName="status"
                  class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white text-sm focus-visible:outline-none transition-colors"
                >
                  <option [value]="ActivityStatus.Doing">Em Andamento</option>
                  <option [value]="ActivityStatus.Done">Concluído</option>
                </select>
              </div>
            </div>

            <!-- Blocker Toggle / Checkbox -->
            <div class="flex items-center space-x-3 p-3 bg-white/5 border border-white/5 rounded-lg">
              <input
                id="isBlocked"
                type="checkbox"
                formControlName="isBlocked"
                class="w-4 h-4 text-[var(--color-accent-brand)] bg-[var(--color-bg-tertiary)] border-white/10 rounded focus:ring-[var(--color-accent-brand)] focus:ring-2"
              />
              <label for="isBlocked" class="text-sm font-medium text-gray-200 cursor-pointer selection:bg-none">
                Esta atividade está bloqueada por algum impedimento?
              </label>
            </div>

            <!-- Submit Buttons -->
            <div class="pt-4 flex gap-3">
              <button
                type="submit"
                [disabled]="activityForm.invalid || isLockedOut"
                class="flex-1 py-3 bg-[var(--color-accent-brand)] hover:opacity-90 disabled:opacity-50 text-white rounded-lg text-sm font-semibold transition-all shadow-md shadow-indigo-500/20 cursor-pointer"
              >
                {{ isEditMode ? 'Salvar Alterações' : 'Salvar Atividade' }}
              </button>
              <button
                type="button"
                routerLink="/parceiro/atividades"
                class="px-6 py-3 bg-white/5 hover:bg-white/10 border border-white/10 text-white rounded-lg text-sm font-semibold transition-colors cursor-pointer"
              >
                Cancelar
              </button>
            </div>

          </form>
        </div>

      </div>
    </div>
  `
})
export class ActivityLogComponent implements OnInit {
  private readonly dailyActivityService = inject(DailyActivityService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);

  readonly ActivityStatus = ActivityStatus;

  isEditMode = false;
  activityId: string | null = null;
  isLockedOut = false;

  activityForm = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    theme: ['', [Validators.required]],
    content: ['', [Validators.required]],
    referenceDate: [this.getTodayDateString(), [Validators.required]],
    status: [ActivityStatus.Doing as ApiActivityStatus, [Validators.required]],
    isBlocked: [false]
  });

  async ngOnInit() {
    await this.checkLockout();
    this.route.paramMap.subscribe(async params => {
      this.activityId = params.get('id');
      if (this.activityId) {
        this.isEditMode = true;
        await this.loadActivity(this.activityId);
      }
    });
  }

  getTodayDateString(): string {
    const today = new Date();
    return today.toISOString().split('T')[0];
  }

  async checkLockout() {
    const partnerId = this.authService.getUserId();
    if (partnerId) {
      try {
        this.isLockedOut = await this.dailyActivityService.getLockoutStatus(partnerId);
        if (this.isLockedOut) {
          this.activityForm.disable();
        }
      } catch (e) {
        console.error('Error checking lockout status', e);
      }
    }
  }

  async loadActivity(id: string) {
    try {
      const act = await this.dailyActivityService.getActivityById(id);
      this.activityForm.patchValue({
        title: act.title,
        theme: act.theme,
        content: act.content,
        referenceDate: act.referenceDate ? act.referenceDate.split('T')[0] : this.getTodayDateString(),
        status: act.status,
        isBlocked: act.isBlocked
      });

      // If the activity is already published or locked out, disable form
      if (act.isPublished || this.isLockedOut) {
        this.activityForm.disable();
      }
    } catch (e) {
      console.error('Error loading activity details', e);
      alert('Erro ao carregar detalhes da atividade.');
      this.router.navigate(['/parceiro/atividades']);
    }
  }

  async onSubmit() {
    if (this.activityForm.invalid || this.isLockedOut) return;

    const partnerId = this.authService.getUserId();
    if (!partnerId) {
      alert('Usuário não autenticado.');
      return;
    }

    const formVal = this.activityForm.getRawValue();

    if (this.isEditMode && this.activityId) {
      const req: UpdateDailyActivityRequest = {
        partnerId,
        title: formVal.title!,
        theme: formVal.theme!,
        content: formVal.content!,
        referenceDate: formVal.referenceDate!,
        status: formVal.status!,
        isBlocked: !!formVal.isBlocked
      };
      try {
        await this.dailyActivityService.updateActivity(this.activityId, req);
        alert('Atividade atualizada com sucesso!');
        this.router.navigate(['/parceiro/atividades']);
      } catch (e) {
        console.error(e);
        alert('Erro ao atualizar atividade. Verifique se o período de edição está bloqueado.');
      }
    } else {
      const req: CreateDailyActivityRequest = {
        partnerId,
        title: formVal.title!,
        theme: formVal.theme!,
        content: formVal.content!,
        referenceDate: formVal.referenceDate!,
        status: formVal.status!,
        isBlocked: !!formVal.isBlocked
      };
      try {
        await this.dailyActivityService.logActivity(req);
        alert('Atividade registrada com sucesso!');
        this.router.navigate(['/parceiro/atividades']);
      } catch (e) {
        console.error(e);
        alert('Erro ao registrar atividade. Verifique se o período de edição está bloqueado.');
      }
    }
  }
}
