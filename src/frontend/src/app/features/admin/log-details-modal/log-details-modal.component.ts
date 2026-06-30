import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EmailAuditLogDto } from '../../../api/models/email-audit-log-dto';

@Component({
  selector: 'app-log-details-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './log-details-modal.component.html',
  styleUrl: './log-details-modal.component.css'
})
export class LogDetailsModalComponent {
  @Input({ required: true }) log!: EmailAuditLogDto;
  @Output() close = new EventEmitter<void>();
}
