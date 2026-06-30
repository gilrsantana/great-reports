import { TestBed } from '@angular/core/testing';
import { UserListComponent } from './user-list.component';
import { UserService } from '../../../core/services/user.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('UserListComponent', () => {
  let component: UserListComponent;
  let mockUserService: any;

  beforeEach(() => {
    mockUserService = {
      getUsers: vi.fn().mockResolvedValue([])
    };

    TestBed.configureTestingModule({
      imports: [UserListComponent],
      providers: [
        { provide: UserService, useValue: mockUserService },
        { provide: ActivatedRoute, useValue: { params: of({}) } }
      ]
    });

    const fixture = TestBed.createComponent(UserListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
