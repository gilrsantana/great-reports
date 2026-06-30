import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { UserService } from '../../../core/services/user.service';

@Component({
  selector: 'app-user-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './user-register.component.html',
  styleUrl: './user-register.component.css'
})
export class UserRegisterComponent {
  private readonly fb = inject(FormBuilder);
  private readonly userService = inject(UserService);
  private readonly router = inject(Router);

  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<boolean>(false);

  readonly registerForm = this.fb.group({
    displayName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    role: ['', Validators.required]
  });

  async onSubmit() {
    if (this.registerForm.invalid) return;

    this.loading.set(true);
    this.error.set(null);
    this.success.set(false);

    const providerId = localStorage.getItem('active_provider_id');
    if (!providerId) {
      this.error.set('Nenhum Provedor ativo selecionado. Defina um Provedor no painel primeiro.');
      this.loading.set(false);
      return;
    }

    const { displayName, email, role } = this.registerForm.value;

    try {
      await this.userService.registerUser({
        providerCompanyId: providerId,
        displayName: displayName!,
        email: email!,
        role: role!
      });
      
      this.success.set(true);
      this.registerForm.reset({
        displayName: '',
        email: '',
        role: ''
      });

      // Redirect after short delay
      setTimeout(() => {
        this.router.navigate(['/admin/usuarios']);
      }, 1500);

    } catch (err: any) {
      console.error(err);
      const errMsg = err?.error?.detail || err?.message || 'Falha ao registrar usuário.';
      this.error.set(errMsg);
    } finally {
      this.loading.set(false);
    }
  }
}
