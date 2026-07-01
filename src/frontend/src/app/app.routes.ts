import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { AdminLayoutComponent } from './features/admin/admin-layout.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'confirm-email',
    loadComponent: () => import('./features/auth/email-confirmation.component').then(m => m.EmailConfirmationComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: '',
        loadComponent: () => import('./features/admin/admin-dashboard.component').then(m => m.AdminDashboardComponent)
      },
      {
        path: 'usuarios',
        loadComponent: () => import('./features/admin/user-list/user-list.component').then(m => m.UserListComponent)
      },
      {
        path: 'usuarios/novo',
        loadComponent: () => import('./features/admin/user-register/user-register.component').then(m => m.UserRegisterComponent)
      },
      {
        path: 'provedores/detalhes',
        loadComponent: () => import('./features/admin/provider-details/provider-details.component').then(m => m.ProviderDetailsComponent)
      },
      {
        path: 'provedores/novo',
        loadComponent: () => import('./features/admin/provider-register/provider-register.component').then(m => m.ProviderRegisterComponent)
      },
      {
        path: 'clientes',
        loadComponent: () => import('./features/admin/client-list/client-list.component').then(m => m.ClientListComponent)
      },
      {
        path: 'clientes/novo',
        loadComponent: () => import('./features/admin/client-register/client-register.component').then(m => m.ClientRegisterComponent)
      },
      {
        path: 'projetos',
        loadComponent: () => import('./features/admin/project-list/project-list.component').then(m => m.ProjectListComponent)
      },
      {
        path: 'projetos/novo',
        loadComponent: () => import('./features/admin/project-register/project-register.component').then(m => m.ProjectRegisterComponent)
      },
      {
        path: 'logs-email',
        loadComponent: () => import('./features/admin/email-audit-list/email-audit-list.component').then(m => m.EmailAuditListComponent)
      }
    ]
  },
  {
    path: 'minha-conta',
    loadComponent: () => import('./features/account/account-management.component').then(m => m.AccountManagementComponent),
    canActivate: [authGuard]
  },
  {
    path: 'lider/grupos',
    loadComponent: () => import('./features/group-leader/group-list/group-list.component').then(m => m.GroupListComponent),
    canActivate: [authGuard]
  },
  {
    path: 'lider/grupos/novo',
    loadComponent: () => import('./features/group-leader/group-register/group-register.component').then(m => m.GroupRegisterComponent),
    canActivate: [authGuard]
  },
  {
    path: 'lider/grupos/:id',
    loadComponent: () => import('./features/group-leader/group-details/group-details.component').then(m => m.GroupDetailsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'parceiro/atividades',
    loadComponent: () => import('./features/partner/activity-history/activity-history.component').then(m => m.ActivityHistoryComponent),
    canActivate: [authGuard]
  },
  {
    path: 'parceiro/atividades/registrar',
    loadComponent: () => import('./features/partner/activity-log/activity-log.component').then(m => m.ActivityLogComponent),
    canActivate: [authGuard]
  },
  {
    path: 'parceiro/atividades/editar/:id',
    loadComponent: () => import('./features/partner/activity-log/activity-log.component').then(m => m.ActivityLogComponent),
    canActivate: [authGuard]
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];
