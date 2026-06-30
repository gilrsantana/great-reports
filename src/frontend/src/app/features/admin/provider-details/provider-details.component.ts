import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { CompanyService } from '../../../core/services/company.service';
import { ProviderDetailsDto } from '../../../api/models/provider-details-dto';

@Component({
  selector: 'app-provider-details',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './provider-details.component.html',
  styleUrl: './provider-details.component.css'
})
export class ProviderDetailsComponent implements OnInit {
  private readonly companyService = inject(CompanyService);

  readonly provider = signal<ProviderDetailsDto | null>(null);
  readonly loading = signal<boolean>(true);

  ngOnInit() {
    this.loadProvider();
  }

  async loadProvider() {
    this.loading.set(true);
    const providerId = localStorage.getItem('active_provider_id');
    if (!providerId) {
      this.provider.set(null);
      this.loading.set(false);
      return;
    }

    try {
      const data = await this.companyService.getProviderDetails(providerId);
      this.provider.set(data);
    } catch (err) {
      console.error(err);
      this.provider.set(null);
    } finally {
      this.loading.set(false);
    }
  }
}
