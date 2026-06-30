---
name: frontend-feature-scaffolding
description: Scaffold a complete frontend feature slice in Angular, including core models, HTTP services using firstValueFrom, components using Signals, lazy routing, and Tailwind themes.
---

# Skill: Scaffolding a Frontend Feature Slice in Angular

This skill guides the assistant through creating a unified frontend feature slice.

---

## Steps

### 1. Create Feature Models
- Create `src/app/core/models/{feature}.models.ts`.
- Define type interfaces matching the backend API responses.
  ```typescript
  export interface ItemResponse {
    id: string;
    name: string;
    description: string;
    createdAt: string;
  }

  export interface CreateItemRequest {
    name: string;
    description: string;
  }
  ```

---

### 2. Implement the HTTP Service
- Create `src/app/core/services/{feature}.service.ts`.
- Service must use the `providedIn: 'root'` decorator and property-inject `HttpClient`.
- Standardize on `firstValueFrom` to support async/await patterns in feature components:
  ```typescript
  import { Injectable, inject } from '@angular/core';
  import { HttpClient } from '@angular/common/http';
  import { Observable, firstValueFrom } from 'rxjs';
  import { ItemResponse, CreateItemRequest } from '../models/{feature}.models';
  import { environment } from '../../../environments/environment';

  @Injectable({
    providedIn: 'root'
  })
  export class ItemService {
    private readonly http = inject(HttpClient);
    private readonly apiBase = `${environment.apiUrl}/api/items`;

    async getAll(): Promise<ItemResponse[]> {
      return await firstValueFrom(
        this.http.get<ItemResponse[]>(this.apiBase)
      );
    }

    async create(request: CreateItemRequest): Promise<string> {
      return await firstValueFrom(
        this.http.post<string>(this.apiBase, request)
      );
    }
  }
  ```

---

### 3. Create Feature Components
- Navigate to `src/app/features/{feature}/`.
- Create list and detail components:
  - Folder `features/{feature}/{feature}-list/`
  - Folder `features/{feature}/{feature}-detail/`
- Every component must be standalone.
- Use Signals (`signal`, `computed`) for handling component state:
  ```typescript
  import { Component, OnInit, inject, signal } from '@angular/core';
  import { ItemService } from '../../../core/services/item.service';
  import { ItemResponse } from '../../../core/models/item.models';

  @Component({
    selector: 'app-item-list',
    standalone: true,
    imports: [],
    templateUrl: './item-list.component.html',
    styleUrl: './item-list.component.css'
  })
  export class ItemListComponent implements OnInit {
    private readonly itemService = inject(ItemService);
    
    readonly items = signal<ItemResponse[]>([]);
    readonly isLoading = signal<boolean>(true);

    async ngOnInit() {
      await this.loadItems();
    }

    async loadItems() {
      this.isLoading.set(true);
      try {
        const data = await this.itemService.getAll();
        this.items.set(data);
      } catch (err) {
        console.error('Failed to load items', err);
      } finally {
        this.isLoading.set(false);
      }
    }
  }
  ```

---

### 4. Apply Tailwind CSS Theme Styles
- In component CSS or HTML, compose classes referencing the custom variables.
- Example component CSS:
  ```css
  .container {
    background-color: var(--color-bg-primary);
    color: var(--color-text-primary);
    border: 1px solid var(--border-color);
    transition: var(--transition-smooth);
  }
  ```

---

### 5. Register Lazy Routing
- Open `app.routes.ts`.
- Add the route utilizing dynamic imports:
  ```typescript
  {
    path: 'items',
    loadComponent: () => import('./features/item/item-list/item-list.component').then(m => m.ItemListComponent)
  }
  ```
