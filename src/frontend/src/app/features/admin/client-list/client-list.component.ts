import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CompanyService } from '../../../core/services/company.service';
import { ClientCompany } from '../../../core/models/client-company.models';

@Component({
  selector: 'app-client-list',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './client-list.component.html',
  styleUrl: './client-list.component.css'
})
export class ClientListComponent implements OnInit {
  private readonly companyService = inject(CompanyService);

  readonly clients = signal<ClientCompany[]>([]);
  readonly loading = signal<boolean>(true);

  // Pagination
  readonly page = signal<number>(1);
  readonly pageSize = signal<number>(8);
  readonly totalItems = signal<number>(0);
  readonly totalPages = signal<number>(0);

  ngOnInit() {
    this.loadClients();
  }

  async loadClients() {
    this.loading.set(true);
    const providerId = localStorage.getItem('active_provider_id');
    if (!providerId) {
      this.clients.set([]);
      this.loading.set(false);
      return;
    }

    try {
      const response = await this.companyService.getClientCompanies(
        providerId,
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
}
