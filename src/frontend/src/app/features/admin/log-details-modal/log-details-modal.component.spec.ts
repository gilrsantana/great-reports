import { TestBed } from '@angular/core/testing';
import { LogDetailsModalComponent } from './log-details-modal.component';
import { describe, it, expect, beforeEach } from 'vitest';

describe('LogDetailsModalComponent', () => {
  let component: LogDetailsModalComponent;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [LogDetailsModalComponent]
    });

    const fixture = TestBed.createComponent(LogDetailsModalComponent);
    component = fixture.componentInstance;
    component.log = {
      id: '123',
      recipient: 'test@email.com',
      subject: 'Test Subject',
      body: 'Test Body',
      success: true,
      sentAt: '2026-06-30T21:00:00Z',
      errorMessage: ''
    };
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
