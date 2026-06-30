import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CompanyService } from '../../../core/services/company.service';

@Component({
  selector: 'app-client-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="p-6 min-h-screen bg-[var(--color-bg-primary)] text-white px-4 font-['Inter']">
      
      <div class="max-w-2xl mx-auto space-y-6">
        
        <!-- Header -->
        <div class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md">
          <div class="flex justify-between items-center">
            <div>
              <h1 class="text-3xl font-extrabold tracking-tight font-['Outfit'] text-white">
                Cadastrar Empresa Cliente
              </h1>
              <p class="text-xs text-[var(--color-text-secondary)] mt-2 uppercase tracking-wider">
                Adicione uma nova empresa cliente sob a administração do seu Provedor.
              </p>
            </div>
            <button routerLink="/admin/clientes" class="px-3.5 py-1.5 bg-white/5 hover:bg-white/10 border border-white/10 text-gray-300 rounded-lg text-sm transition-colors cursor-pointer">
              Voltar
            </button>
          </div>
        </div>

        <!-- Form Card -->
        <div class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md">
          
          <form [formGroup]="registerForm" (ngSubmit)="onSubmit()" class="space-y-4">
            
            <!-- Nome da Empresa Cliente -->
            <div>
              <label for="name" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
                Nome da Empresa Cliente
              </label>
              <input
                id="name"
                type="text"
                formControlName="name"
                class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white placeholder-gray-500 text-sm focus-visible:ring-2 focus-visible:ring-[var(--color-accent-brand)] focus-visible:outline-none transition-colors"
                placeholder="Ex: ACME Tech Corp"
              />
              <div *ngIf="registerForm.get('name')?.touched && registerForm.get('name')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
                O nome da empresa cliente é obrigatório.
              </div>
            </div>

            <!-- Messages -->
            <div *ngIf="error()" class="p-3 bg-[var(--color-accent-rose)]/10 border border-[var(--color-accent-rose)]/25 rounded-lg text-xs text-[var(--color-accent-rose)] font-medium">
              {{ error() }}
            </div>
            <div *ngIf="success()" class="p-3 bg-[var(--color-accent-emerald)]/10 border border-[var(--color-accent-emerald)]/25 rounded-lg text-xs text-[var(--color-accent-emerald)] font-medium">
              Empresa Cliente cadastrada com sucesso! Redirecionando...
            </div>

            <!-- Submit Button -->
            <div class="flex justify-end pt-4">
              <button
                type="submit"
                [disabled]="registerForm.invalid || loading()"
                class="px-6 py-2.5 bg-[var(--color-accent-brand)] hover:opacity-90 disabled:opacity-50 text-white font-semibold rounded-lg text-sm transition-all shadow-md shadow-indigo-500/10 hover:shadow-indigo-500/20 flex items-center gap-2 cursor-pointer disabled:cursor-not-allowed"
              >
                <svg *ngIf="loading()" class="animate-spin h-4 w-4 text-white" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" fill="none"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <span>Cadastrar Cliente</span>
              </button>
            </div>
          </form>

        </div>

      </div>

    </div>
  `
})
export class ClientRegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly companyService = inject(CompanyService);
  private readonly router = inject(Router);

  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<boolean>(false);

  readonly registerForm = this.fb.group({
    name: ['', Validators.required]
  });

  async onSubmit() {
    if (this.registerForm.invalid) return;

    this.loading.set(true);
    this.error.set(null);
    this.success.set(false);

    const providerId = localStorage.getItem('active_provider_id');
    if (!providerId) {
      this.error.set('Nenhum Provedor ativo configurado. Defina um Provedor ativo no painel primeiro.');
      this.loading.set(false);
      return;
    }

    const { name } = this.registerForm.value;

    try {
      await this.companyService.registerClient({
        providerCompanyId: providerId,
        name: name!
      });
      
      this.success.set(true);
      this.registerForm.reset();

      setTimeout(() => {
        this.router.navigate(['/admin/clientes']);
      }, 1500);

    } catch (err: any) {
      console.error(err);
      const errMsg = err?.error?.detail || err?.message || 'Falha ao cadastrar cliente.';
      this.error.set(errMsg);
    } finally {
      this.loading.set(false);
    }
  }
}
