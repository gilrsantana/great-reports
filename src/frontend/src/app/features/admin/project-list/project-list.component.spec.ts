import { TestBed } from '@angular/core/testing';
import { ProjectListComponent } from './project-list.component';
import { CompanyService } from '../../../core/services/company.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('ProjectListComponent', () => {
  let component: ProjectListComponent;
  let mockCompanyService: any;

  beforeEach(() => {
    mockCompanyService = {
      getProjects: vi.fn().mockResolvedValue({ items: [], totalItems: 0 })
    };

    TestBed.configureTestingModule({
      imports: [ProjectListComponent],
      providers: [
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: ActivatedRoute, useValue: { params: of({}) } }
      ]
    });

    const fixture = TestBed.createComponent(ProjectListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
