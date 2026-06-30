---
name: frontend-admin-design-system
description: High-quality, premium design guidelines for the Angular admin interface. Defines typography, colors, layout components, data grids, state-driven widgets, transitions, and accessibility.
---

# Skill: Angular Admin Design System

This skill outlines the design tokens, layouts, styling rules, and template standards for building a premium, modern, and highly polished admin interface in Great Reports.

---

## 1. Typography & Fonts

To maintain a clean and professional appearance, use the following font system:

- **Primary Headings**: `Outfit` (sans-serif, geometric, highly legible, letter-spacing: tight).
- **Body & Controls**: `Inter` (sans-serif, optimized for user interfaces, neutral, highly legible at small sizes).
- **Data & Monospace**: `JetBrains Mono` (for UUIDs, CNPJs, timestamps, code blocks, or numeric grids).

### Font Setup in `index.html`
Ensure the fonts are imported from Google Fonts:
```html
<link rel="preconnect" href="https://fonts.googleapis.com">
<link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
<link href="https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&family=Outfit:wght@400;500;600;700;800&family=JetBrains+Mono:wght@400;500&display=swap" rel="stylesheet">
```

---

## 2. Color Palette (Dark-Theme Aesthetics)

Great Reports utilizes a premium dark theme built on sleek slates, subtle gradients, and glassmorphism.

Define the Tailwind variables in `src/frontend/src/styles.css`:
```css
@theme {
  /* Surfaces */
  --color-bg-primary: #0B0F19;      /* Ultra-dark slate */
  --color-bg-secondary: #111827;    /* Elevated panel grey */
  --color-bg-tertiary: #1F2937;     /* High elevation / inputs */
  
  /* Borders */
  --color-border-glass: rgba(255, 255, 255, 0.08);
  --color-border-hover: rgba(255, 255, 255, 0.15);

  /* Accents */
  --color-accent-brand: #4F46E5;    /* Indigo base */
  --color-accent-emerald: #10B981;  /* Success states */
  --color-accent-rose: #F43F5E;     /* Error/destruct states */
  --color-accent-amber: #F59E0B;    /* Warning states */
  
  /* Text */
  --color-text-primary: #FFFFFF;    /* Pure title white */
  --color-text-secondary: #94A3B8;  /* Muted body text */
  --color-text-dim: #64748B;       /* Low-contrast details */
  
  /* Motion */
  --transition-smooth: all 300ms cubic-bezier(0.4, 0, 0.2, 1);
  --transition-slow: all 450ms cubic-bezier(0.16, 1, 0.3, 1);
}
```

---

## 3. UI Aesthetics & Micro-Animations

- **Glassmorphism**: Panels, sidebars, and modals should use translucent surfaces with light borders:
  `bg-white/5 border border-white/10 backdrop-blur-md`
- **Interactive Lift**: Cards and buttons should scale subtly and glow on hover:
  `hover:-translate-y-0.5 hover:shadow-lg hover:shadow-indigo-500/10 hover:border-white/15 transition-all duration-300`
- **Accessible Focus States**: Do NOT use default browser rings. Custom outline styling is mandatory:
  `focus-visible:ring-2 focus-visible:ring-accent-brand focus-visible:ring-offset-2 focus-visible:ring-offset-bg-primary focus-visible:outline-none`

---

## 4. Core Layout Components

### A. Admin Dashboard Layout (Main Shell)
The layout consists of a fixed Sidebar on the left, a Top Navbar, and a scrollable main content container.
```html
<div class="flex h-screen bg-[var(--color-bg-primary)] overflow-hidden font-['Inter']">
  <!-- Sidebar -->
  <aside class="w-64 bg-[var(--color-bg-secondary)] border-r border-[var(--color-border-glass)] flex flex-col">
    <div class="p-6 border-b border-[var(--color-border-glass)]">
      <span class="text-xl font-bold tracking-tight text-white font-['Outfit']">Great Reports</span>
    </div>
    <nav class="flex-1 p-4 space-y-1 overflow-y-auto">
      <a href="/admin" class="flex items-center px-4 py-2.5 text-sm font-medium text-white bg-white/5 border border-white/5 rounded-lg">
        Painel
      </a>
    </nav>
  </aside>

  <!-- Content Container -->
  <div class="flex-1 flex flex-col overflow-hidden">
    <!-- Top Header -->
    <header class="h-16 border-b border-[var(--color-border-glass)] bg-[var(--color-bg-secondary)]/50 backdrop-blur-md flex items-center justify-between px-8">
      <span class="text-sm font-semibold text-[var(--color-text-secondary)]">Painel Administrativo</span>
      <div class="flex items-center gap-4">
        <!-- Profile / Notifications -->
      </div>
    </header>

    <!-- Main Content View -->
    <main class="flex-1 overflow-y-auto p-8">
      <router-outlet></router-outlet>
    </main>
  </div>
</div>
```

### B. Analytics Metrics (KPI Panel)
Metrics should be structured inside a grid, using bold typefaces and dynamic micro-trends.
```html
<div class="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
  <div class="bg-white/5 border border-[var(--color-border-glass)] rounded-xl p-6 backdrop-blur-md hover:border-[var(--color-border-hover)] transition-all duration-300">
    <span class="text-xs font-semibold text-[var(--color-text-secondary)] uppercase tracking-wider">Total de Clientes</span>
    <div class="flex items-baseline gap-2 mt-2">
      <span class="text-3xl font-bold font-['Outfit'] text-white">124</span>
      <span class="text-xs text-[var(--color-accent-emerald)] font-medium">+12%</span>
    </div>
  </div>
</div>
```

### C. Data Table / Grid
Data tables must have clear headers, hover transitions per row, and use monospace for identifiers.
```html
<div class="overflow-x-auto rounded-xl border border-[var(--color-border-glass)] bg-white/5 backdrop-blur-md">
  <table class="w-full text-left border-collapse text-sm">
    <thead>
      <tr class="border-b border-[var(--color-border-glass)] text-[var(--color-text-secondary)] font-medium">
        <th class="py-4 px-6">Nome</th>
        <th class="py-4 px-6">Identificador</th>
        <th class="py-4 px-6 text-right">Ações</th>
      </tr>
    </thead>
    <tbody class="divide-y divide-white/5">
      <tr class="hover:bg-white/5 transition-colors duration-200">
        <td class="py-4 px-6 font-semibold text-white">Empresa Alpha</td>
        <td class="py-4 px-6 text-[var(--color-text-secondary)] font-mono text-xs">d290f1ee-6c54-4b01-90e6-d701748f0851</td>
        <td class="py-4 px-6 text-right">
          <button class="px-3 py-1 bg-[var(--color-accent-brand)] hover:opacity-90 rounded text-xs text-white font-medium transition-all">
            Detalhes
          </button>
        </td>
      </tr>
    </tbody>
  </table>
</div>
```

---

## 5. References & Integration

This skill defines the styling specifications. It integrates with:
- **Core Syntax & Architecture**: Must align with the code structure rules and standalone constraints documented in [10-frontend-framework-definitions](../10-frontend-framework-definitions/SKILL.md).
- **Procedural Steps**: The steps to create and link components using these styles are detailed in [00-frontend-scaffold-playbook](../00-frontend-scaffold-playbook/SKILL.md).
