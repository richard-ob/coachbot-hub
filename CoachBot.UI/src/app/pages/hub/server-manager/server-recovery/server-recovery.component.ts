import { Component, ViewChild, EventEmitter, Output } from '@angular/core';
import { SwalPortalTargets, SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { ServerService } from '@pages/hub/shared/services/server.service';
import { Server } from '@pages/hub/shared/model/server.model';

@Component({
    selector: 'app-server-recovery',
    templateUrl: './server-recovery.component.html'
})
export class ServerRecoveryComponent {

    @ViewChild('recoverServerModal') recoverServerModal: SwalComponent;
    @Output() serverRecovered = new EventEmitter<void>();
    servers: Server[];
    isLoading = true;
    isRecovering = false;

    constructor(
        private serverService: ServerService,
        public readonly swalTargets: SwalPortalTargets
    ) { }

    openModal() {
        this.getDeactivatedServers();
        this.recoverServerModal.fire();
    }

    getDeactivatedServers() {
        this.serverService.getDeactivatedServers().subscribe((servers) => {
            this.servers = servers;
            this.isLoading = false;
            this.isRecovering = false;
        });
    }

    recoverServer(serverId: number): void {
        this.isRecovering = true;
        this.serverService.reactivateServer(serverId).subscribe(() => {
            this.getDeactivatedServers();
            this.serverRecovered.emit();
        });
    }

}
