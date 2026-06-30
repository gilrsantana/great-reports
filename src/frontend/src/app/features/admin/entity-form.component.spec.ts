import { TestBed } from '@angular/core/testing';
import { EntityFormComponent } from './entity-form.component';
import { CompanyService } from '../../core/services/company.service';
import { UserService } from '../../core/services/user.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { FormsModule } from '@angular/forms';

describe('EntityFormComponent', () => {
  let component: EntityFormComponent;
  let mockCompanyService: any;
  let mockUserService: any;

  beforeEach(() => {
    mockCompanyService = {
      registerProvider: vi.fn().mockResolvedValue({}),
      registerClientCompany: vi.fn().mockResolvedValue({}),
      registerProject: vi.fn().mockResolvedValue({}),
      createClientContact: vi.fn().mockResolvedValue({})
    };
    mockUserService = {
      registerUser: vi.fn().mockResolvedValue({})
    };

    TestBed.configureTestingModule({
      imports: [EntityFormComponent, FormsModule],
      providers: [
        { provide: CompanyService, useValue: mockCompanyService },
        { provide: UserService, useValue: mockUserService }
      ]
    });

    const fixture = TestBed.createComponent(EntityFormComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
