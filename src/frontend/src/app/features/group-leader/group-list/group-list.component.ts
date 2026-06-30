import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GroupService } from '../../../core/services/group.service';
import { AuthService } from '../../../core/services/auth.service';
import { GroupDto } from '../../../api/models/group-dto';

@Component({
  selector: 'app-group-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './group-list.component.html',
  styleUrl: './group-list.component.css'
})
export class GroupListComponent implements OnInit {
  private readonly groupService = inject(GroupService);
  private readonly authService = inject(AuthService);
  readonly groups = signal<GroupDto[]>([]);
  readonly loading = signal<boolean>(true);

  ngOnInit() {
    this.loadGroups();
  }

  async loadGroups() {
    try {
      const role = this.authService.getRole();
      const groupLeaderId = role === 'GroupLeader' ? (this.authService.getUserId() || undefined) : undefined;
      const res = await this.groupService.getGroups(groupLeaderId);
      this.groups.set(res);
    } catch (e) {
      console.error(e);
    } finally {
      this.loading.set(false);
    }
  }
}
