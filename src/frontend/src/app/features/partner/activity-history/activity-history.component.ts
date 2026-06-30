import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DailyActivityService } from '../../../core/services/daily-activity.service';
import { AuthService } from '../../../core/services/auth.service';
import { DailyActivityDto } from '../../../api/models/daily-activity-dto';
import { ActivityStatus as ApiActivityStatus } from '../../../api/models/activity-status';

export const ActivityStatus = {
  Doing: 0,
  Done: 1
} as const;

@Component({
  selector: 'app-activity-history',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './activity-history.component.html',
  styleUrl: './activity-history.component.css'
})
export class ActivityHistoryComponent implements OnInit {
  private readonly dailyActivityService = inject(DailyActivityService);
  private readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);

  readonly ActivityStatus = ActivityStatus;

  activities: DailyActivityDto[] = [];
  isLockedOut = false;

  filterForm = this.fb.group({
    title: [''],
    theme: [''],
    status: [''],
    date: ['']
  });

  async ngOnInit() {
    await this.checkLockout();
    await this.loadActivities();
  }

  async checkLockout() {
    const partnerId = this.authService.getUserId();
    if (partnerId) {
      try {
        this.isLockedOut = await this.dailyActivityService.getLockoutStatus(partnerId);
      } catch (e) {
        console.error('Error checking lockout', e);
      }
    }
  }

  async loadActivities() {
    const partnerId = this.authService.getUserId();
    if (!partnerId) return;

    const val = this.filterForm.value;

    try {
      this.activities = await this.dailyActivityService.getActivities({
        partnerId,
        title: val.title || undefined,
        theme: val.theme || undefined,
        status: val.status ? (parseInt(val.status) as ApiActivityStatus) : undefined,
        date: val.date || undefined,
        page: 1,
        pageSize: 100
      });
    } catch (e) {
      console.error(e);
      alert('Erro ao carregar histórico de atividades.');
    }
  }

  async applyFilters() {
    await this.loadActivities();
  }

  async clearFilters() {
    this.filterForm.reset({
      title: '',
      theme: '',
      status: '',
      date: ''
    });
    await this.loadActivities();
  }
}
