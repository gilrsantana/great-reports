import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { GroupService } from '../../../core/services/group.service';
import { GroupDto } from '../../../api/models/group-dto';
import { ScheduledEmailConfigComponent } from '../scheduled-email-config/scheduled-email-config.component';

@Component({
  selector: 'app-group-details',
  standalone: true,
  imports: [CommonModule, RouterModule, ScheduledEmailConfigComponent],
  template: `
    <div class="p-6 min-h-screen bg-[var(--color-bg-primary)] text-white px-4 font-['Inter']">
      <div class="max-w-6xl mx-auto space-y-6">
        
        <!-- Header Panel -->
        <div class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md flex flex-col md:flex-row justify-between items-start md:items-center gap-4 animate-fade-in">
          <div>
            <div class="flex items-center gap-3">
              <h1 class="text-3xl font-extrabold tracking-tight font-['Outfit'] text-white">
                {{ group()?.name || 'Detalhes do Grupo' }}
              </h1>
              <span class="px-2.5 py-0.5 bg-indigo-500/10 text-indigo-300 border border-indigo-500/20 rounded-full text-xs font-semibold">
                Timezone: {{ group()?.timezone }}
              </span>
            </div>
            <p class="text-xs text-[var(--color-text-secondary)] mt-2 uppercase tracking-wider">
              Painel de administração e agendamentos de e-mail do grupo.
            </p>
          </div>
          <button routerLink="/lider/grupos" class="px-3.5 py-1.5 bg-white/5 hover:bg-white/10 border border-white/10 text-gray-300 rounded-lg text-sm transition-colors cursor-pointer">
            Voltar aos Grupos
          </button>
        </div>

        <!-- Info Grid -->
        <div *ngIf="group() as g" class="grid grid-cols-1 md:grid-cols-3 gap-6">
          <div class="bg-white/5 border border-white/10 rounded-xl p-6 backdrop-blur-md col-span-1 space-y-4">
            <h3 class="text-sm font-bold text-white uppercase tracking-wider border-b border-white/10 pb-2">Informações Gerais</h3>
            
            <div>
              <p class="text-xs text-[var(--color-text-secondary)]">ID do Grupo</p>
              <p class="text-sm font-mono text-gray-300">{{ g.id }}</p>
            </div>
            
            <div>
              <p class="text-xs text-[var(--color-text-secondary)]">ID da Empresa Cliente</p>
              <p class="text-sm font-mono text-gray-300">{{ g.clientCompanyId }}</p>
            </div>

            <div>
              <p class="text-xs text-[var(--color-text-secondary)]">ID do Projeto</p>
              <p class="text-sm font-mono text-gray-300">{{ g.projectId }}</p>
            </div>
          </div>

          <!-- Configuration Panel -->
          <div class="md:col-span-2">
            <app-scheduled-email-config [groupId]="g.id" [clientCompanyId]="g.clientCompanyId"></app-scheduled-email-config>
          </div>
        </div>

      </div>
    </div>
  `
})
export class GroupDetailsComponent implements OnInit {
  private readonly groupService = inject(GroupService);
  private readonly route = inject(ActivatedRoute);

  readonly group = signal<GroupDto | null>(null);

  ngOnInit() {
    this.route.paramMap.subscribe(async params => {
      const id = params.get('id');
      if (id) {
        try {
          const g = await this.groupService.getGroupById(id);
          this.group.set(g);
        } catch (e) {
          console.error(e);
        }
      }
    });
  }
}
