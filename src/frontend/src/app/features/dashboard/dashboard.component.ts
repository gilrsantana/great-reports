import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  template: `
    <div class="p-6 bg-[var(--color-bg-primary)] text-white min-h-screen">
      <h1 class="text-3xl font-semibold mb-4 text-[var(--color-accent-brand)]">Painel de Relatórios</h1>
      <p class="text-gray-300">Bem-vindo ao Great Reports.</p>
    </div>
  `
})
export class DashboardComponent {}
