import { Component } from '@angular/core';

@Component({
  selector: 'app-login',
  standalone: true,
  template: `
    <div class="flex items-center justify-center min-h-screen bg-[var(--color-bg-primary)] text-white">
      <div class="p-8 rounded-lg bg-[var(--color-bg-secondary)] shadow-md w-full max-w-md">
        <h2 class="text-2xl font-bold text-center mb-6 text-[var(--color-accent-brand)]">Great Reports - Login</h2>
        <p class="text-center text-gray-400">Página de login inicial (Em breve)</p>
      </div>
    </div>
  `
})
export class LoginComponent {}
