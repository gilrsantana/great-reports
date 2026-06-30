import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CompanyService } from '../../../core/services/company.service';

@Component({
  selector: 'app-provider-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './provider-register.component.html',
  styleUrl: './provider-register.component.css'
})
export class ProviderRegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly companyService = inject(CompanyService);
  private readonly router = inject(Router);

  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<boolean>(false);

  readonly registerForm = this.fb.group({
    name: ['', Validators.required],
    taxId: ['', [Validators.required, Validators.pattern(/^\d{14}$/)]]
  });

  async onSubmit() {
    if (this.registerForm.invalid) return;

    this.loading.set(true);
    this.error.set(null);
    this.success.set(false);

    const { name, taxId } = this.registerForm.value;

    try {
      const providerId = await this.companyService.registerProvider({
        name: name!,
        taxId: taxId!
      });
      
      // Save newly registered provider ID as active
      localStorage.setItem('active_provider_id', providerId);

      this.success.set(true);
      this.registerForm.reset();

      setTimeout(() => {
        this.router.navigate(['/admin/provedores/detalhes']);
      }, 1500);

    } catch (err: any) {
      console.error(err);
      const errMsg = err?.error?.detail || err?.message || 'Falha ao cadastrar provedor.';
      this.error.set(errMsg);
    } finally {
      this.loading.set(false);
    }
  }
}
