import { Component } from '@angular/core';
import { Server } from '../model/server';
import { ServerService } from '../shared/services/server.service';
import { Region } from '../model/region';
import { RegionService } from '../shared/services/region.service';

@Component({
    selector: 'app-servers',
    templateUrl: './servers.component.html'
})
export class ServersComponent {

    newServer: Server = new Server();
    servers: Server[];
    regions: Region[];

    constructor(private serverService: ServerService, private regionService: RegionService) {
        this.serverService.getServers().subscribe(servers => this.servers = servers);
        this.regionService.getRegions().subscribe(regions => this.regions = regions);
    }

    addServer() {
        this.serverService.addServer(this.newServer).subscribe(() => {
            this.serverService.getServers().subscribe(servers => this.servers = servers);
            this.newServer = new Server();
        });
    }

    removeServer(serverId: number) {
        serverId++; // serverId's aren't zero indexed
        this.serverService.removeServer(serverId).subscribe(() => {
            this.serverService.getServers().subscribe(servers => this.servers = servers);
        });
    }
}
