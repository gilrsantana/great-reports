# Rule: Tailwind CSS v4 Theme and Styling Standards

## Metadata
- **ID**: RULE-011-TAILWIND-V4-STYLING
- **Scope**: Frontend (Angular templates & CSS)
- **Target Types**: HTML, CSS, TS Component styling
- **Status**: Active

## Overview
This rule defines the styling standards for the frontend application using Tailwind CSS v4. It enforces the use of custom theme tokens, modern typography, glassmorphism aesthetics, smooth motion transitions, and responsive grid layouts to preserve a premium visual quality.

---

## 1. Tailwind v4 Theme Tokens (`@theme` in `styles.css`)
Developers must always use the variables configured in the `@theme` block of the main stylesheet. Never hardcode arbitrary colors or fonts in the inline HTML class lists.

### A. Color Palette
- **Backgrounds**:
  - Primary canvas: `bg-bg-primary` (mapped to `{PrimaryBgColor}`)
  - Accent cards/sections: `bg-bg-secondary` (mapped to `{SecondaryBgColor}`)
  - Dark elements/overlays: `bg-bg-dark` (mapped to `{DarkBgColor}`)
- **Text**:
  - Primary headings/body: `text-text-primary` (mapped to `{PrimaryTextColor}`)
  - Secondary metadata/labels: `text-text-secondary` (mapped to `{SecondaryTextColor}`)
  - Light mode inverse: `text-text-light` (mapped to `{LightTextColor}`)
- **Accent Theme Color**:
  - Brand highlight: `text-accent-brand` (mapped to `{AccentColor}`)
  - Brand hover state: `text-accent-brand-hover` (mapped to `{AccentHoverColor}`)

### B. Typography
- **Sans-Serif (Body & UI)**:
  - Font Stack: `font-sans` (uses Google Font '{SansFont}')
- **Serif (Headings & Titles)**:
  - Font Stack: `font-serif` (uses Google Font '{SerifFont}')

---

## 2. Micro-Animations and Transitions
To maintain a responsive and alive user experience:
- **Hover Transitions**: Always use the smooth custom transitions and transformation offsets when designing interactive elements:
  - Transition duration & curve: `transition-all duration-400 ease-[cubic-bezier(0.16,1,0.3,1)]`
  - Action offset on hover: `-translate-y-1.5 shadow-lg border-accent-brand`
- **Button Standards**:
  - Use the preset `.btn`, `.btn-primary`, `.btn-secondary`, and `.btn-brand` helper utility styles to keep buttons aligned with standard heights, paddings, and hover styles.

---

## 3. Premium Interface Patterns (Glassmorphism & Modals)
- **Glass Navbar & Panels**: Use the glass opacity background combined with a light border and backdrop blur:
  - Glass combination: `bg-bg-primary/75 backdrop-blur-md border-b border-black/5`
- **Overlay & Modals**:
  - Backdrop overlay: `fixed inset-0 bg-bg-dark/40 backdrop-blur-xs z-50 flex items-center justify-center`
  - Modal content containers must slide up and fade in smoothly on load.

---

## 4. Example Premium Component Template
```html
<div class="premium-card p-6 flex flex-col justify-between h-full bg-bg-primary border border-black/10 rounded-xl transition-all duration-400 ease-[cubic-bezier(0.16,1,0.3,1)] hover:-translate-y-1.5 hover:shadow-lg hover:border-accent-brand">
  <div>
    <span class="badge badge-brand bg-accent-brand/15 text-accent-brand mb-3">Featured Item</span>
    <h3 class="text-xl font-serif text-text-primary mb-2">Architectural Showroom</h3>
    <p class="text-sm text-text-secondary font-sans leading-relaxed">
      Curated showcase of architectural design models constructed using premium materials.
    </p>
  </div>
  <div class="mt-6 flex justify-between items-center">
    <span class="text-lg font-serif text-accent-brand font-semibold">$1,200.00</span>
    <a routerLink="/details" class="btn btn-sm btn-primary">View Details</a>
  </div>
</div>
```
