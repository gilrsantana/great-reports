import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { GroupService } from '../../../core/services/group.service';
import { CompanyService } from '../../../core/services/company.service';
import { AuthService } from '../../../core/services/auth.service';
import { CreateGroupRequest } from '../../../api/models/create-group-request';

@Component({
  selector: 'app-group-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="p-6 min-h-screen bg-[var(--color-bg-primary)] text-white px-4 font-['Inter']">
      <div class="max-w-2xl mx-auto space-y-6">
        <div class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md">
          <div class="flex justify-between items-center">
            <div>
              <h1 class="text-3xl font-extrabold tracking-tight font-['Outfit'] text-white">Criar Novo Grupo</h1>
              <p class="text-xs text-[var(--color-text-secondary)] mt-2 uppercase tracking-wider">
                Defina as configurações do grupo de trabalho.
              </p>
            </div>
            <button routerLink="/lider/grupos" class="px-3.5 py-1.5 bg-white/5 hover:bg-white/10 border border-white/10 text-gray-300 rounded-lg text-sm transition-colors cursor-pointer">
              Voltar
            </button>
          </div>
        </div>

        <div class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md">
          <form [formGroup]="groupForm" (ngSubmit)="onSubmit()" class="space-y-4">
            <div>
              <label for="name" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Nome do Grupo
              </label>
              <input
                id="name"
                type="text"
                formControlName="name"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white placeholder-gray-500 text-sm focus-visible:ring-2 focus-visible:ring-[var(--color-accent-brand)] focus-visible:outline-none transition-colors"
                placeholder="Ex: Grupo de Suporte"
              />
              <div *ngIf="groupForm.get('name')?.touched && groupForm.get('name')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
                O nome do grupo é obrigatório.
              </div>
            </div>

            <div>
              <label for="clientCompanyId" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Empresa Cliente
              </label>
              <select
                id="clientCompanyId"
                formControlName="clientCompanyId"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white text-sm focus-visible:outline-none transition-colors"
              >
                <option value="">Selecione uma empresa...</option>
                <option *ngFor="let c of companies" [value]="c.id">{{ c.name }}</option>
              </select>
              <div *ngIf="groupForm.get('clientCompanyId')?.touched && groupForm.get('clientCompanyId')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
                A empresa cliente é obrigatória.
              </div>
            </div>

            <div>
              <label for="projectId" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Projeto (ID)
              </label>
              <input
                id="projectId"
                type="text"
                formControlName="projectId"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white placeholder-gray-500 text-sm focus-visible:ring-2 focus-visible:ring-[var(--color-accent-brand)] focus-visible:outline-none transition-colors"
                placeholder="Insira o UUID do Projeto"
              />
              <div *ngIf="groupForm.get('projectId')?.touched && groupForm.get('projectId')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
                O ID do projeto é obrigatório.
              </div>
            </div>

            <div>
              <label for="timezone" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Fuso Horário
              </label>
              <select
                id="timezone"
                formControlName="timezone"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white text-sm focus-visible:outline-none transition-colors"
              >
                <option value="America/Sao_Paulo">America/Sao_Paulo (Horário de Brasília)</option>
                <option value="UTC">UTC (Universal Time Coordinated)</option>
                <option value="America/New_York">America/New_York (Eastern Time)</option>
                <option value="Europe/London">Europe/London (Greenwich Mean Time)</option>
              </select>
            </div>

            <div>
              <label for="groupLeaderId" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                ID do Líder do Grupo
              </label>
              <input
                id="groupLeaderId"
                type="text"
                formControlName="groupLeaderId"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white placeholder-gray-500 text-sm focus-visible:ring-2 focus-visible:ring-[var(--color-accent-brand)] focus-visible:outline-none transition-colors"
                placeholder="Insira o UUID do Líder do Grupo"
              />
              <div *ngIf="groupForm.get('groupLeaderId')?.touched && groupForm.get('groupLeaderId')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
                O ID do líder do grupo é obrigatório.
              </div>
            </div>

            <div class="pt-4">
              <button
                type="submit"
                [disabled]="groupForm.invalid"
                class="w-full py-3 bg-[var(--color-accent-brand)] hover:opacity-90 disabled:opacity-50 text-white rounded-lg text-sm font-semibold transition-all shadow-md shadow-indigo-500/20 cursor-pointer"
              >
                Salvar Grupo
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `
})
export class GroupRegisterComponent implements OnInit {
  private readonly groupService = inject(GroupService);
  private readonly companyService = inject(CompanyService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  companies: any[] = [];

  groupForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    clientCompanyId: ['', Validators.required],
    projectId: ['3sa2f5e8-0b5c-4d7a-8f1b-c3e6d2b4a5f9', Validators.required], // default dummy UUID
    timezone: ['America/Sao_Paulo', Validators.required],
    groupLeaderId: ['', Validators.required]
  });

  ngOnInit() {
    this.loadCompanies();
    const currentUserId = this.authService.getUserId();
    if (currentUserId) {
      this.groupForm.patchValue({ groupLeaderId: currentUserId });
    }
  }

  async loadCompanies() {
    try {
      const res = await this.companyService.getClientCompanies('', 1, 100);
      this.companies = res.items || [];
    } catch (e) {
      console.error(e);
    }
  }

  async onSubmit() {
    if (this.groupForm.invalid) return;
    const req: CreateGroupRequest = {
      name: this.groupForm.value.name!,
      clientCompanyId: this.groupForm.value.clientCompanyId!,
      projectId: this.groupForm.value.projectId!,
      timezone: this.groupForm.value.timezone!,
      groupLeaderId: this.groupForm.value.groupLeaderId!
    };
    try {
      await this.groupService.createGroup(req);
      alert('Grupo criado com sucesso!');
      this.router.navigate(['/lider/grupos']);
    } catch (e) {
      console.error(e);
      alert('Erro ao criar o grupo. Verifique os dados e tente novamente.');
    }
  }
}
