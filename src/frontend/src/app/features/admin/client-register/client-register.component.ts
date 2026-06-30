import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { CompanyService } from '../../../core/services/company.service';

@Component({
  selector: 'app-client-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './client-register.component.html',
  styleUrl: './client-register.component.css'
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
