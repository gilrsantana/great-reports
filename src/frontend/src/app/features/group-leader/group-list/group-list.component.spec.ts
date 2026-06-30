import { TestBed } from '@angular/core/testing';
import { GroupListComponent } from './group-list.component';
import { GroupService } from '../../../core/services/group.service';
import { AuthService } from '../../../core/services/auth.service';
import { describe, it, expect, beforeEach, vi } from 'vitest';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';

describe('GroupListComponent', () => {
  let component: GroupListComponent;
  let mockGroupService: any;
  let mockAuthService: any;

  beforeEach(() => {
    mockGroupService = {
      getGroups: vi.fn().mockResolvedValue([])
    };
    mockAuthService = {
      getRole: vi.fn().mockReturnValue('GroupLeader'),
      getUserId: vi.fn().mockReturnValue('leader-123')
    };

    TestBed.configureTestingModule({
      imports: [GroupListComponent],
      providers: [
        { provide: GroupService, useValue: mockGroupService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: ActivatedRoute, useValue: { params: of({}) } }
      ]
    });

    const fixture = TestBed.createComponent(GroupListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
