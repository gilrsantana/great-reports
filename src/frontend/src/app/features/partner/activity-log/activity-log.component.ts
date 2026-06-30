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
  templateUrl: './activity-log.component.html',
  styleUrl: './activity-log.component.css'
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
