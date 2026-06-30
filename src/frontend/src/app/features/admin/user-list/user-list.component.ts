import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { UserDto } from '../../../api/models/user-dto';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.css'
})
export class UserListComponent implements OnInit {
  private readonly userService = inject(UserService);

  readonly users = signal<UserDto[]>([]);
  readonly loading = signal<boolean>(true);

  ngOnInit() {
    this.loadUsers();
  }

  async loadUsers() {
    this.loading.set(true);
    const providerId = localStorage.getItem('active_provider_id');
    if (!providerId) {
      this.users.set([]);
      this.loading.set(false);
      return;
    }

    try {
      const data = await this.userService.getUsers(providerId);
      this.users.set(data);
    } catch (err) {
      console.error(err);
      this.users.set([]);
    } finally {
      this.loading.set(false);
    }
  }
}
