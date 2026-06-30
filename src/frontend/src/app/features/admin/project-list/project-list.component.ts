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
  template: `
    <div class="p-6 min-h-screen bg-[var(--color-bg-primary)] text-white font-['Inter']">
      <div class="max-w-6xl mx-auto space-y-6">
        <div class="flex justify-between items-center bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md">
          <h1 class="text-3xl font-extrabold tracking-tight font-['Outfit'] text-white">Gerenciar Projetos</h1>
          <button routerLink="/admin/projetos/novo" class="px-4 py-2.5 bg-[var(--color-accent-brand)] hover:opacity-90 text-white rounded-lg text-sm font-semibold transition-all shadow-md shadow-indigo-500/20 cursor-pointer">
            + Novo Projeto
          </button>
        </div>
        <div *ngIf="loading()" class="py-12 flex justify-center items-center text-gray-400">
          <span>Carregando projetos...</span>
        </div>
        <div *ngIf="!loading() && projects().length === 0" class="py-16 text-center text-gray-400 border border-dashed border-white/10 rounded-lg">
          <p class="font-semibold text-white">Nenhum projeto cadastrado</p>
        </div>
        <div *ngIf="!loading() && projects().length > 0" class="overflow-x-auto">
          <table class="w-full text-left border-collapse text-sm">
            <thead>
              <tr class="border-b border-white/10 text-gray-400 font-medium">
                <th class="py-3 px-4">Nome</th>
                <th class="py-3 px-4">Empresa Cliente</th>
              </tr>
            </thead>
            <tbody class="divide-y divide-white/5">
              <tr *ngFor="let proj of projects()" class="hover:bg-white/5 transition-colors">
                <td class="py-3 px-4 font-semibold text-white">{{ proj.name }}</td>
                <td class="py-3 px-4 text-gray-300 font-medium">{{ proj.clientCompanyId }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  `
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
