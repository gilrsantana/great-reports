import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CompanyService } from '../../../core/services/company.service';
import { RegisterProjectRequest } from '../../../api/models/register-project-request';
import { Router } from '@angular/router';

@Component({
  selector: 'app-project-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
   template: `
    <div class="p-6 min-h-screen bg-[var(--color-bg-primary)] text-white font-['Inter']">
      <div class="max-w-2xl mx-auto space-y-6">
        <h1 class="text-3xl font-extrabold tracking-tight font-['Outfit'] text-white">Registrar Projeto</h1>
        <form [formGroup]="registerForm" (ngSubmit)="onSubmit()" class="space-y-4">
          <div>
            <label class="block text-sm font-medium mb-1" for="name">Nome do Projeto</label>
            <input id="name" type="text" class="w-full rounded border border-gray-300 p-2 bg-white/5 text-white" formControlName="name" />
          </div>
          <div>
            <label class="block text-sm font-medium mb-1" for="clientCompanyId">Empresa Cliente</label>
            <select id="clientCompanyId" class="w-full rounded border border-gray-300 p-2 bg-white/5 text-white" formControlName="clientCompanyId">
              <option *ngFor="let c of (clientCompanies$ | async)?.items" [value]="c.id">{{ c.name }}</option>
            </select>
          </div>
          <button type="submit" class="px-4 py-2 bg-[var(--color-accent-brand)] hover:opacity-90 text-white rounded">
            Salvar Projeto
          </button>
        </form>
      </div>
    </div>
    `,
   styles: [],
})
export class ProjectRegisterComponent {
  private readonly companyService = inject(CompanyService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  // Form definition
  registerForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    clientCompanyId: ['', Validators.required]
  });

  clientCompanies$ = this.companyService.getClientCompanies('', 1, 100);

  async onSubmit() {
    if (this.registerForm.invalid) return;
    const req: RegisterProjectRequest = {
      name: this.registerForm.value.name!,
      clientCompanyId: this.registerForm.value.clientCompanyId!,
      description: ''
    };
    try {
      await this.companyService.registerProject(req);
      // Success feedback in BR-Portuguese
      alert('Projeto registrado com sucesso!');
      this.router.navigate(['/admin/projetos']);
    } catch (e) {
      console.error(e);
      alert('Erro ao registrar o projeto. Por favor, tente novamente.');
    }
  }
}
