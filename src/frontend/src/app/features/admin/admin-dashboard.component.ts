import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyService } from '../../core/services/company.service';
import { ClientCompany } from '../../core/models/client-company.models';
import { EntityFormComponent } from './entity-form.component';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, EntityFormComponent],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.css'
})
export class AdminDashboardComponent implements OnInit {
  private readonly companyService = inject(CompanyService);

  // States
  clients = signal<ClientCompany[]>([]);
  selectedProviderId = signal<string>('');
  providerName = signal<string>('');
  providerTaxId = signal<string>('');
  loading = signal<boolean>(false);

  // Pagination states
  page = signal<number>(1);
  pageSize = signal<number>(8);
  totalItems = signal<number>(0);
  totalPages = signal<number>(0);

  // Input states
  providerInputId = '';
  selectedClientCompanyId = '';

  // Form Modal configurations
  isFormOpen = false;
  formType: 'provider' | 'client' | 'contact' | 'project' | 'user' = 'provider';

  ngOnInit() {
    // Try to load default provider from local storage if existing
    const savedId = localStorage.getItem('active_provider_id');
    if (savedId) {
      this.providerInputId = savedId;
      this.selectedProviderId.set(savedId);
      this.loadProviderDetails();
      this.loadClients();
    }
  }

  changeProvider() {
    if (this.providerInputId && this.providerInputId.length === 36) {
      this.selectedProviderId.set(this.providerInputId);
      localStorage.setItem('active_provider_id', this.providerInputId);
      this.page.set(1);
      this.loadProviderDetails();
      this.loadClients();
    } else {
      this.clearProvider();
    }
  }

  clearProvider() {
    this.selectedProviderId.set('');
    this.providerName.set('');
    this.providerTaxId.set('');
    this.clients.set([]);
    this.totalItems.set(0);
    this.totalPages.set(0);
    localStorage.removeItem('active_provider_id');
  }

  async loadProviderDetails() {
    try {
      const details = await this.companyService.getProviderDetails(this.selectedProviderId());
      this.providerName.set(details.name);
      this.providerTaxId.set(details.taxId);
    } catch (err) {
      console.error('Failed to load provider details', err);
      this.providerName.set('Provedor Desconhecido');
    }
  }

  async loadClients() {
    if (!this.selectedProviderId()) return;

    this.loading.set(true);
    try {
      const response = await this.companyService.getClientCompanies(
        this.selectedProviderId(),
        this.page(),
        this.pageSize()
      );
      this.clients.set(response.items);
      this.totalItems.set(response.totalCount);
      this.totalPages.set(response.totalPages);
    } catch (err) {
      console.error(err);
      this.clients.set([]);
    } finally {
      this.loading.set(false);
    }
  }

  prevPage() {
    if (this.page() > 1) {
      this.page.update(p => p - 1);
      this.loadClients();
    }
  }

  nextPage() {
    if (this.page() < this.totalPages()) {
      this.page.update(p => p + 1);
      this.loadClients();
    }
  }

  openForm(type: 'provider' | 'client' | 'contact' | 'project' | 'user', clientCompanyId = '') {
    this.formType = type;
    this.selectedClientCompanyId = clientCompanyId;
    
    // Auto-save the provider details if we just saved a new provider
    if (type === 'provider') {
      this.selectedClientCompanyId = '';
    }

    this.isFormOpen = true;
  }
}
