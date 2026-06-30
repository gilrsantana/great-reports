import { Component, OnInit, signal, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-email-confirmation',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './email-confirmation.component.html',
  styleUrl: './email-confirmation.component.css'
})
export class EmailConfirmationComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly authService = inject(AuthService);

  loading = signal<boolean>(true);
  success = signal<boolean>(false);
  error = signal<string | null>(null);

  ngOnInit() {
    this.route.queryParams.subscribe(async params => {
      const email = params['email'];
      const token = params['token'];

      if (!email || !token) {
        this.error.set('Parâmetros de confirmação ausentes na URL.');
        this.loading.set(false);
        return;
      }

      try {
        await this.authService.confirmEmail(email, token);
        this.success.set(true);
      } catch (err: any) {
        console.error(err);
        const errMsg = err?.error?.detail || err?.message || 'Token inválido ou expirado.';
        this.error.set(errMsg);
      } finally {
        this.loading.set(false);
      }
    });
  }
}
