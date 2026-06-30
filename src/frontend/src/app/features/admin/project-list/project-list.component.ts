import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CompanyService } from '../../../core/services/company.service';
export interface ProjectDto {
  id?: string;
  name?: string;
  clientCompanyId?: string;
}

@Component({
  selector: 'app-project-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './project-list.component.html',
  styleUrl: './project-list.component.css'
})
export class ProjectListComponent implements OnInit {
  private readonly companyService = inject(CompanyService);
  readonly projects = signal<ProjectDto[]>([]);
  readonly loading = signal<boolean>(true);

  ngOnInit() {
    this.loadProjects();
  }

  async loadProjects() {
    // TODO: replace with actual service call when GET endpoint is available
    this.projects.set([]);
    this.loading.set(false);
  }
}
