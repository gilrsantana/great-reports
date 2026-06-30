import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-account-management',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './account-management.component.html',
  styleUrl: './account-management.component.css'
})
export class AccountManagementComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);

  readonly email = signal<string | null>(this.authService.getEmail());
  readonly role = signal<string | null>(this.authService.getRole());
  readonly userId = signal<string | null>(this.authService.getUserId());

  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);
  readonly success = signal<boolean>(false);

  readonly passwordForm = this.fb.group({
    currentPassword: ['', Validators.required],
    newPassword: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', Validators.required]
  }, {
    validators: (group) => {
      const newPass = group.get('newPassword')?.value;
      const confirmPass = group.get('confirmPassword')?.value;
      return newPass === confirmPass ? null : { mismatch: true };
    }
  });

  async onPasswordSubmit() {
    if (this.passwordForm.invalid) return;

    this.loading.set(true);
    this.error.set(null);
    this.success.set(false);

    const { currentPassword, newPassword } = this.passwordForm.value;

    try {
      await this.authService.changePassword(currentPassword!, newPassword!);
      this.success.set(true);
      this.passwordForm.reset();
    } catch (err: any) {
      console.error(err);
      const errMsg = err?.error?.detail || err?.message || 'Erro ao alterar a senha. Verifique se a senha atual está correta.';
      this.error.set(errMsg);
    } finally {
      this.loading.set(false);
    }
  }
}
