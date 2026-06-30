import { Component, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  template: `
    <div class="flex items-center justify-center min-h-screen bg-[var(--color-bg-primary)] text-white px-4 font-['Inter']">
      <div class="p-8 border border-white/10 rounded-xl bg-white/5 backdrop-blur-md shadow-2xl w-full max-w-md hover:border-white/15 hover:shadow-indigo-500/5 transition-all duration-300">
        
        <div class="text-center mb-8">
          <h2 class="text-3xl font-extrabold tracking-tight font-['Outfit'] text-white">
            Great Reports
          </h2>
          <p class="text-xs text-[var(--color-text-secondary)] mt-2 uppercase tracking-wider">
            Faça login para gerenciar seus relatórios
          </p>
        </div>

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()" class="space-y-6">
          <!-- Email Input -->
          <div>
            <label for="email" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider mb-2">
              E-mail
            </label>
            <input
              id="email"
              type="email"
              formControlName="email"
              class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white placeholder-gray-500 text-sm focus-visible:ring-2 focus-visible:ring-[var(--color-accent-brand)] focus-visible:outline-none transition-colors"
              placeholder="seu.email@provedor.com"
            />
            <div *ngIf="loginForm.get('email')?.touched && loginForm.get('email')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
              <span *ngIf="loginForm.get('email')?.errors?.['required']">O e-mail é obrigatório.</span>
              <span *ngIf="loginForm.get('email')?.errors?.['email']">Insira um endereço de e-mail válido.</span>
            </div>
          </div>

          <!-- Password Input -->
          <div>
            <div class="flex justify-between items-center mb-2">
              <label for="password" class="block text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider">
                Senha
              </label>
              <a routerLink="/auth/recuperar-senha" class="text-xs text-[var(--color-accent-brand)] hover:underline font-medium">
                Esqueceu a senha?
              </a>
            </div>
            <input
              id="password"
              type="password"
              formControlName="password"
              class="w-full bg-[var(--color-bg-tertiary)] border border-white/10 focus:border-[var(--color-accent-brand)] rounded-lg px-4 py-2.5 text-white placeholder-gray-500 text-sm focus-visible:ring-2 focus-visible:ring-[var(--color-accent-brand)] focus-visible:outline-none transition-colors"
              placeholder="••••••••"
            />
            <div *ngIf="loginForm.get('password')?.touched && loginForm.get('password')?.invalid" class="text-xs text-[var(--color-accent-rose)] mt-1.5 font-medium">
              <span *ngIf="loginForm.get('password')?.errors?.['required']">A senha é obrigatória.</span>
              <span *ngIf="loginForm.get('password')?.errors?.['minlength']">A senha deve ter pelo menos 8 caracteres.</span>
            </div>
          </div>

          <!-- Global Error Display -->
          <div *ngIf="error()" class="p-3 bg-[var(--color-accent-rose)]/10 border border-[var(--color-accent-rose)]/25 rounded-lg text-xs text-[var(--color-accent-rose)] font-medium">
            {{ error() }}
          </div>

          <!-- Submit Button -->
          <button
            type="submit"
            [disabled]="loginForm.invalid || loading()"
            class="w-full py-3 bg-[var(--color-accent-brand)] hover:opacity-90 disabled:opacity-50 text-white font-semibold rounded-lg text-sm transition-all shadow-md shadow-indigo-500/10 hover:shadow-indigo-500/20 flex items-center justify-center gap-2 cursor-pointer disabled:cursor-not-allowed"
          >
            <svg *ngIf="loading()" class="animate-spin h-4 w-4 text-white" viewBox="0 0 24 24">
              <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4" fill="none"></circle>
              <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
            </svg>
            <span>{{ loading() ? 'Entrando...' : 'Entrar' }}</span>
          </button>
        </form>

      </div>
    </div>
  `
})
export class LoginComponent {
  private readonly fb = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly loading = signal<boolean>(false);
  readonly error = signal<string | null>(null);

  readonly loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  async onSubmit() {
    if (this.loginForm.invalid) return;

    this.loading.set(true);
    this.error.set(null);

    const { email, password } = this.loginForm.value;

    try {
      await this.authService.login(email!, password!);
      // Redirect based on role or to admin dashboard by default
      const role = this.authService.getRole();
      if (role === 'Manager' || role === 'Maintainer') {
        this.router.navigate(['/admin']);
      } else {
        this.router.navigate(['/dashboard']);
      }
    } catch (err: any) {
      console.error(err);
      const errMsg = err?.error?.detail || err?.message || 'E-mail ou senha incorretos.';
      this.error.set(errMsg);
    } finally {
      this.loading.set(false);
    }
  }
}
