import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { GroupService } from '../../../core/services/group.service';
import { GroupDto } from '../../../api/models/group-dto';
import { ScheduledEmailConfigComponent } from '../scheduled-email-config/scheduled-email-config.component';

@Component({
  selector: 'app-group-details',
  standalone: true,
  imports: [CommonModule, RouterModule, ScheduledEmailConfigComponent],
  templateUrl: './group-details.component.html',
  styleUrl: './group-details.component.css'
})
export class GroupDetailsComponent implements OnInit {
  private readonly groupService = inject(GroupService);
  private readonly route = inject(ActivatedRoute);

  readonly group = signal<GroupDto | null>(null);

  ngOnInit() {
    this.route.paramMap.subscribe(async params => {
      const id = params.get('id');
      if (id) {
        try {
          const g = await this.groupService.getGroupById(id);
          this.group.set(g);
        } catch (e) {
          console.error(e);
        }
      }
    });
  }
}
