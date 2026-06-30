import { Component, Input, Output, EventEmitter, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyService } from '../../core/services/company.service';
import { UserService } from '../../core/services/user.service';

@Component({
  selector: 'app-entity-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './entity-form.component.html',
  styleUrl: './entity-form.component.css'
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
