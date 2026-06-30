---
name: frontend-scaffold-playbook
description: Master playbook to guide the chronological creation of new frontend features (API generation, services, signal-based components, routing, and design system) by linking individual skills.
---

# Playbook: Scaffolding a New Frontend Feature

This playbook guides you step-by-step through creating a complete frontend feature slice in Great Reports. Follow the phases below sequentially.

---

## The Scaffolding Sequence Checklist

### 🏁 Phase 1: API Generation (API-First Design)
Great Reports uses API-first development. All API interaction services and data transfer models are generated from the backend OpenAPI specification.

1. Verify the backend OpenAPI spec (`openapi.json` or `openapi.yaml`) is up to date.
2. Run the client generation command inside the frontend root:
   ```bash
   npm run generate:api
   ```
3. Verify the generated services and models are created under the output api directory (e.g. `src/app/api/services/` and `src/app/api/models/`).
   - 👉 *Refer to rules:* [10-frontend-framework-definitions](../10-frontend-framework-definitions/SKILL.md)

---

### ⚙️ Phase 2: Create Core Services Wrapper
Create a custom core service under `src/app/core/services/` to wrap the generated API client. This service maps generated models and converts RxJS Observables to Promises to allow cleaner async/await workflows in components.

1. Create `src/app/core/services/{feature}.service.ts`.
2. Property-inject the generated API service using the `inject()` function.
3. Use RxJS `firstValueFrom` to return Promises for transactional methods:

```typescript
import { Injectable, inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ReportsApiService } from 'src/app/api/services/reports-api.service';
import { ReportModel } from 'src/app/api/models/report-model';

@Injectable({
  providedIn: 'root'
})
export class ReportsService {
  private readonly reportsApi = inject(ReportsApiService);

  async getAllReports(): Promise<ReportModel[]> {
    return await firstValueFrom(
      this.reportsApi.getReportsList()
    );
  }

  async createReport(data: { name: string; content: string }): Promise<string> {
    return await firstValueFrom(
      this.reportsApi.postCreateReport({ body: data })
    );
  }
}
```

---

### 🖥️ Phase 3: Create UI Components
Scaffold feature views inside `src/app/features/{feature}/`.

1. Generate list and detail components:
   - `src/app/features/{feature}/{feature}-list/`
   - `src/app/features/{feature}/{feature}-detail/`
2. Every component must be standalone. Do NOT define `standalone: true` or `changeDetection: ChangeDetectionStrategy.OnPush` (defaults in Angular v20+ / v22+).
3. Use signals (`signal()`, `computed()`) for local state:

```typescript
import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportsService } from '../../../core/services/reports.service';
import { ReportModel } from '../../../api/models/report-model';

@Component({
  selector: 'app-report-list',
  imports: [CommonModule],
  templateUrl: './report-list.component.html',
  styleUrl: './report-list.component.css'
})
export class ReportListComponent implements OnInit {
  private readonly reportsService = inject(ReportsService);

  readonly reports = signal<ReportModel[]>([]);
  readonly loading = signal<boolean>(true);

  async ngOnInit() {
    await this.loadReports();
  }

  async loadReports() {
    this.loading.set(true);
    try {
      const data = await this.reportsService.getAllReports();
      this.reports.set(data);
    } catch (error) {
      console.error('Failed to load reports', error);
    } finally {
      this.loading.set(false);
    }
  }
}
```

---

### 🎨 Phase 4: Apply Admin Styling
Align all component markups with the visual standards of the admin interface.

1. Use fonts: `Outfit` for headings and titles, `Inter` for regular text, and `JetBrains Mono` for IDs.
2. Apply the dark glassmorphic styling:
   - Panel wrapper: `bg-white/5 border border-white/10 backdrop-blur-md rounded-xl`
   - Hover transition: `hover:-translate-y-0.5 hover:shadow-lg hover:shadow-indigo-500/10 hover:border-white/15 transition-all duration-300`
   - Focus ring: `focus-visible:ring-2 focus-visible:ring-accent-brand focus-visible:ring-offset-2 focus-visible:ring-offset-bg-primary focus-visible:outline-none`
   - 👉 *Refer to style specs:* [11-frontend-admin-design-system](../11-frontend-admin-design-system/SKILL.md)

---

### 🔌 Phase 5: Wire Lazy Routing
Register the new components as lazy routes in the central router config.

1. Open `src/app/app.routes.ts`.
2. Add lazy component entries using dynamic imports:
   ```typescript
   export const routes: Routes = [
     {
       path: 'reports',
       loadComponent: () => import('./features/reports/report-list/report-list.component').then(m => m.ReportListComponent)
     }
   ];
   ```

---

### 🧪 Phase 6: Write Unit Tests (Vitest)
Create spec files for the new service and components under the same directory using `.spec.ts` naming. Run tests with:
```bash
npm run test
```

#### A. Service Unit Test Template (`src/app/core/services/{feature}.service.spec.ts`)
```typescript
import { TestBed } from '@angular/core/testing';
import { ReportsService } from './reports.service';
import { ReportsApiService } from 'src/app/api/services/reports-api.service';
import { of, throwError } from 'rxjs';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('ReportsService', () => {
  let service: ReportsService;
  let reportsApiMock: any;

  beforeEach(() => {
    reportsApiMock = {
      getReportsList: vi.fn(),
      postCreateReport: vi.fn()
    };

    TestBed.configureTestingModule({
      providers: [
        ReportsService,
        { provide: ReportsApiService, useValue: reportsApiMock }
      ]
    });

    service = TestBed.inject(ReportsService);
  });

  it('should fetch all reports successfully', async () => {
    const mockData = [{ id: '1', name: 'Report 1' }];
    reportsApiMock.getReportsList.mockReturnValue(of(mockData));

    const result = await service.getAllReports();

    expect(result).toEqual(mockData);
    expect(reportsApiMock.getReportsList).toHaveBeenCalledTimes(1);
  });

  it('should propagate errors when api fails', async () => {
    reportsApiMock.getReportsList.mockReturnValue(throwError(() => new Error('API Error')));

    await expect(service.getAllReports()).rejects.toThrow('API Error');
  });
});
```

#### B. Component Unit Test Template (`src/app/features/{feature}/{feature}-list/{feature}-list.component.spec.ts`)
```typescript
import { TestBed, ComponentFixture } from '@angular/core/testing';
import { ReportListComponent } from './report-list.component';
import { ReportsService } from '../../../core/services/reports.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';

describe('ReportListComponent', () => {
  let component: ReportListComponent;
  let fixture: ComponentFixture<ReportListComponent>;
  let reportsServiceMock: any;

  beforeEach(async () => {
    reportsServiceMock = {
      getAllReports: vi.fn().mockResolvedValue([])
    };

    await TestBed.configureTestingModule({
      imports: [ReportListComponent],
      providers: [
        { provide: ReportsService, useValue: reportsServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ReportListComponent);
    component = fixture.componentInstance;
  });

  it('should load reports on initialization', async () => {
    const mockReports = [{ id: '123', name: 'Sales Q2' }];
    reportsServiceMock.getAllReports.mockResolvedValue(mockReports);

    // Trigger ngOnInit and wait for promise resolutions
    fixture.detectChanges();
    await fixture.whenStable();

    expect(component.reports()).toEqual(mockReports);
    expect(component.loading()).toBe(false);
  });

  it('should handle load failure cleanly', async () => {
    reportsServiceMock.getAllReports.mockRejectedValue(new Error('Load Error'));

    fixture.detectChanges();
    await fixture.whenStable();

    expect(component.reports()).toEqual([]);
    expect(component.loading()).toBe(false);
  });
});
```

---

### ♿ Phase 7: Accessibility Auditing (A11y)
Before committing, check that the new components meet minimum accessibility requirements:

1. HTML structure uses semantic landmarks (`<main>`, `<aside>`, `<header>`, `<article>`).
2. Inputs have associated `<label>` elements or ARIA attributes.
3. Tab navigation is logical, and interactive items have proper focus styling.
4. Color contrast meets the WCAG AA minimum contrast ratio of 4.5:1.
5. All tests must pass AXE accessibility verification.

