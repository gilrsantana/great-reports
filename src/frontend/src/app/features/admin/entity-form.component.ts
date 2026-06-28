import { Component, Input, Output, EventEmitter, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyService } from '../../core/services/company.service';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-entity-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div *ngIf="isOpen" class="fixed inset-0 flex items-center justify-center bg-black/60 backdrop-blur-sm z-50 transition-opacity duration-300">
      <div class="bg-[var(--color-bg-secondary)] border border-white/10 rounded-xl shadow-2xl p-6 w-full max-w-md mx-4 animate-in fade-in zoom-in-95 duration-200">
        
        <div class="flex justify-between items-center mb-6">
          <h2 class="text-xl font-bold text-white">
            {{ getTitle() }}
          </h2>
          <button (click)="close()" class="text-gray-400 hover:text-white transition-colors">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <form (ngSubmit)="submit()" #form="ngForm" class="space-y-4">
          <!-- Error Alert -->
          <div *ngIf="error()" class="p-3 bg-red-500/20 border border-red-500/30 text-red-200 text-sm rounded-lg">
            {{ error() }}
          </div>

          <!-- Success Alert -->
          <div *ngIf="success()" class="p-3 bg-green-500/20 border border-green-500/30 text-green-200 text-sm rounded-lg">
            {{ success() }}
          </div>

          <!-- Provider Fields -->
          <ng-container *ngIf="entityType === 'provider'">
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">Nome do Provedor *</label>
              <input type="text" [(ngModel)]="fields.name" name="name" required class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors" placeholder="Ex: Great Reports Ltda">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">CNPJ / ID Fiscal *</label>
              <input type="text" [(ngModel)]="fields.taxId" name="taxId" required class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors" placeholder="Ex: 00.000.000/0001-00">
            </div>
          </ng-container>

          <!-- Client Company Fields -->
          <ng-container *ngIf="entityType === 'client'">
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">Nome da Empresa Cliente *</label>
              <input type="text" [(ngModel)]="fields.name" name="name" required class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors" placeholder="Ex: ACME Corp">
            </div>
          </ng-container>

          <!-- Project Fields -->
          <ng-container *ngIf="entityType === 'project'">
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">Nome do Projeto *</label>
              <input type="text" [(ngModel)]="fields.name" name="name" required class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors" placeholder="Ex: Redesign Portal">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">Descrição</label>
              <textarea [(ngModel)]="fields.description" name="description" rows="3" class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors" placeholder="Ex: Detalhes sobre o projeto..."></textarea>
            </div>
          </ng-container>

          <!-- User Fields -->
          <ng-container *ngIf="entityType === 'user'">
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">Nome de Exibição *</label>
              <input type="text" [(ngModel)]="fields.displayName" name="displayName" required class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors" placeholder="Ex: João Silva">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">E-mail *</label>
              <input type="email" [(ngModel)]="fields.email" name="email" required email class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors" placeholder="Ex: joao.silva@provedor.com">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">Função / Cargo *</label>
              <select [(ngModel)]="fields.role" name="role" required class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors">
                <option value="Partner" class="bg-[var(--color-bg-secondary)]">Partner (Parceiro)</option>
                <option value="GroupLeader" class="bg-[var(--color-bg-secondary)]">GroupLeader (Líder do Grupo)</option>
                <option value="Manager" class="bg-[var(--color-bg-secondary)]">Manager (Gerente)</option>
                <option value="Maintainer" class="bg-[var(--color-bg-secondary)]">Maintainer (Suporte Técnico)</option>
              </select>
            </div>
          </ng-container>

          <!-- Contact Fields -->
          <ng-container *ngIf="entityType === 'contact'">
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">Nome do Contato *</label>
              <input type="text" [(ngModel)]="fields.name" name="name" required class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors" placeholder="Ex: Maria Souza">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">E-mail *</label>
              <input type="email" [(ngModel)]="fields.email" name="email" required email class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white placeholder-gray-500 focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors" placeholder="Ex: maria.souza@cliente.com">
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-300 mb-1">Tipo de Contato *</label>
              <select [(ngModel)]="fields.contactType" name="contactType" required class="w-full bg-white/5 border border-white/10 rounded-lg px-3 py-2 text-white focus:outline-none focus:border-[var(--color-accent-brand)] transition-colors">
                <option value="Commercial" class="bg-[var(--color-bg-secondary)]">Comercial</option>
                <option value="Tech" class="bg-[var(--color-bg-secondary)]">Técnico</option>
              </select>
            </div>
          </ng-container>

          <!-- Footer Buttons -->
          <div class="flex justify-end gap-3 mt-6 pt-4 border-t border-white/10">
            <button type="button" (click)="close()" [disabled]="loading()" class="px-4 py-2 border border-white/10 hover:bg-white/5 text-gray-300 rounded-lg text-sm transition-colors">
              Cancelar
            </button>
            <button type="submit" [disabled]="!form.valid || loading()" class="px-4 py-2 bg-[var(--color-accent-brand)] hover:opacity-90 disabled:opacity-50 text-white font-semibold rounded-lg text-sm transition-all shadow-md shadow-emerald-500/20">
              {{ loading() ? 'Salvando...' : 'Salvar' }}
            </button>
          </div>

        </form>
      </div>
    </div>
  `
})
export class EntityFormComponent {
  @Input() isOpen = false;
  @Input() entityType: 'provider' | 'client' | 'contact' | 'project' | 'user' = 'provider';
  @Input() providerCompanyId = '';
  @Input() clientCompanyId = '';

  @Output() saved = new EventEmitter<void>();
  @Output() closed = new EventEmitter<void>();

  private readonly companyService = inject(CompanyService);
  private readonly userService = inject(UserService);

  loading = signal(false);
  error = signal<string | null>(null);
  success = signal<string | null>(null);

  fields = {
    name: '',
    taxId: '',
    description: '',
    displayName: '',
    email: '',
    role: 'Partner',
    contactType: 'Commercial'
  };

  getTitle(): string {
    switch (this.entityType) {
      case 'provider': return 'Cadastrar Provedor';
      case 'client': return 'Cadastrar Empresa Cliente';
      case 'project': return 'Cadastrar Novo Projeto';
      case 'user': return 'Cadastrar Novo Usuário';
      case 'contact': return 'Cadastrar Contato Cliente';
    }
  }

  close() {
    this.resetForm();
    this.closed.emit();
  }

  resetForm() {
    this.fields = {
      name: '',
      taxId: '',
      description: '',
      displayName: '',
      email: '',
      role: 'Partner',
      contactType: 'Commercial'
    };
    this.error.set(null);
    this.success.set(null);
    this.loading.set(false);
  }

  async submit() {
    this.loading.set(true);
    this.error.set(null);
    this.success.set(null);

    try {
      if (this.entityType === 'provider') {
        await this.companyService.registerProvider({
          name: this.fields.name,
          taxId: this.fields.taxId
        });
        this.success.set('Provedor cadastrado com sucesso!');
      } else if (this.entityType === 'client') {
        if (!this.providerCompanyId) throw new Error('ID do provedor não informado.');
        await this.companyService.registerClient({
          providerCompanyId: this.providerCompanyId,
          name: this.fields.name
        });
        this.success.set('Empresa cliente cadastrada com sucesso!');
      } else if (this.entityType === 'project') {
        if (!this.clientCompanyId) throw new Error('ID da empresa cliente não informado.');
        await this.companyService.registerProject({
          clientCompanyId: this.clientCompanyId,
          name: this.fields.name,
          description: this.fields.description
        });
        this.success.set('Projeto cadastrado com sucesso!');
      } else if (this.entityType === 'user') {
        if (!this.providerCompanyId) throw new Error('ID do provedor não informado.');
        await this.userService.registerUser({
          providerCompanyId: this.providerCompanyId,
          displayName: this.fields.displayName,
          email: this.fields.email,
          role: this.fields.role
        });
        this.success.set('Usuário cadastrado com sucesso! E-mail de validação enviado.');
      } else if (this.entityType === 'contact') {
        if (!this.clientCompanyId) throw new Error('ID da empresa cliente não informado.');
        await this.companyService.addClientContact(this.clientCompanyId, {
          name: this.fields.name,
          email: this.fields.email,
          contactType: this.fields.contactType
        });
        this.success.set('Contato cliente adicionado com sucesso! E-mail de validação enviado.');
      }

      setTimeout(() => {
        this.saved.emit();
        this.close();
      }, 1500);

    } catch (err: any) {
      console.error(err);
      const errMsg = err?.error?.detail || err?.message || 'Ocorreu um erro ao salvar.';
      this.error.set(errMsg);
      this.loading.set(false);
    }
  }
}
