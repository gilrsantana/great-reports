import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CompanyService } from '../../../core/services/company.service';
import { RegisterProjectRequest } from '../../../api/models/register-project-request';
import { Router } from '@angular/router';

@Component({
  selector: 'app-project-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './project-register.component.html',
  styleUrl: './project-register.component.css'
})
export class ProjectRegisterComponent {
  private readonly companyService = inject(CompanyService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  // Form definition
  registerForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    clientCompanyId: ['', Validators.required]
  });

  clientCompanies$ = this.companyService.getClientCompanies('', 1, 100);

  async onSubmit() {
    if (this.registerForm.invalid) return;
    const req: RegisterProjectRequest = {
      name: this.registerForm.value.name!,
      clientCompanyId: this.registerForm.value.clientCompanyId!,
      description: ''
    };
    try {
      await this.companyService.registerProject(req);
      // Success feedback in BR-Portuguese
      alert('Projeto registrado com sucesso!');
      this.router.navigate(['/admin/projetos']);
    } catch (e) {
      console.error(e);
      alert('Erro ao registrar o projeto. Por favor, tente novamente.');
    }
  }
}
