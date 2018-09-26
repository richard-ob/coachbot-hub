import { Component } from '@angular/core';
import { Server } from '../model/server';
import { ServerService } from '../shared/services/server.service';

@Component({
    selector: 'app-servers',
    templateUrl: './servers.component.html'
})
export class ServersComponent {

    newServer: Server = new Server();
    servers: Server[];

    constructor(private serverService: ServerService) {
        this.serverService.getServers().subscribe(servers => this.servers = servers);
    }

    addServer() {
        this.serverService.addServer(this.newServer).subscribe(complete => {
            this.serverService.getServers().subscribe(servers => this.servers = servers);
            this.newServer = new Server();
        });
    }

    removeServer(serverId: number) {
        serverId++; // serverId's aren't zero indexed
        this.serverService.removeServer(serverId).subscribe(complete => {
            this.serverService.getServers().subscribe(servers => this.servers = servers);
        });
    }
}
