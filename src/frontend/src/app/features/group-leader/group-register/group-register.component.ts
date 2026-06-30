import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { GroupService } from '../../../core/services/group.service';
import { CompanyService } from '../../../core/services/company.service';
import { AuthService } from '../../../core/services/auth.service';
import { CreateGroupRequest } from '../../../api/models/create-group-request';

@Component({
  selector: 'app-group-register',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterModule],
  templateUrl: './group-register.component.html',
  styleUrl: './group-register.component.css'
})
export class GroupRegisterComponent implements OnInit {
  private readonly groupService = inject(GroupService);
  private readonly companyService = inject(CompanyService);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);
  private readonly fb = inject(FormBuilder);

  companies: any[] = [];

  groupForm = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3)]],
    clientCompanyId: ['', Validators.required],
    projectId: ['3sa2f5e8-0b5c-4d7a-8f1b-c3e6d2b4a5f9', Validators.required], // default dummy UUID
    timezone: ['America/Sao_Paulo', Validators.required],
    groupLeaderId: ['', Validators.required]
  });

  ngOnInit() {
    this.loadCompanies();
    const currentUserId = this.authService.getUserId();
    if (currentUserId) {
      this.groupForm.patchValue({ groupLeaderId: currentUserId });
    }
  }

  async loadCompanies() {
    try {
      const res = await this.companyService.getClientCompanies('', 1, 100);
      this.companies = res.items || [];
    } catch (e) {
      console.error(e);
    }
  }

  async onSubmit() {
    if (this.groupForm.invalid) return;
    const req: CreateGroupRequest = {
      name: this.groupForm.value.name!,
      clientCompanyId: this.groupForm.value.clientCompanyId!,
      projectId: this.groupForm.value.projectId!,
      timezone: this.groupForm.value.timezone!,
      groupLeaderId: this.groupForm.value.groupLeaderId!
    };
    try {
      await this.groupService.createGroup(req);
      alert('Grupo criado com sucesso!');
      this.router.navigate(['/lider/grupos']);
    } catch (e) {
      console.error(e);
      alert('Erro ao criar o grupo. Verifique os dados e tente novamente.');
    }
  }
}
