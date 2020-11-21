import { Component, OnInit, ViewChild } from '@angular/core';
import { Server } from '../shared/model/server.model';
import { Region } from '../shared/model/region.model';
import { Country } from '../shared/model/country.model';
import { ServerService } from '../shared/services/server.service';
import { RegionService } from '../shared/services/region.service';
import { CountryService } from '../shared/services/country.service';
import { forkJoin } from 'rxjs';
import { SwalPortalTargets, SwalComponent } from '@sweetalert2/ngx-sweetalert2';

@Component({
  selector: 'app-server-manager',
  templateUrl: './server-manager.component.html',
  styleUrls: ['./server-manager.component.scss']
})
export class ServerManagerComponent {

  @ViewChild('editServerPasswordModal') editServerPasswordModal: SwalComponent;
  @ViewChild('editServerNameModal') editServerNameModal: SwalComponent;
  newServer: Server = new Server();
  servers: Server[];
  regions: Region[];
  countries: Country[];
  serverToEdit: Server;
  isLoading = true;
  isRemoving = false;
  isUpdating = false;

  constructor(
    private serverService: ServerService,
    public readonly swalTargets: SwalPortalTargets
  ) {
    this.getServers();
  }

  getServers() {
    this.isUpdating = true;
    this.serverService.getServers().subscribe((servers) => {
      this.servers = servers;
      this.isUpdating = false;
      this.isLoading = false;
    });
  }

  removeServer(serverId: number) {
    this.isRemoving = true;
    this.serverService.removeServer(serverId).subscribe(() => {
      this.serverService.getServers().subscribe(servers => {
        this.servers = servers;
        this.isRemoving = false;
      });
    });
  }

  editServerPassword(server: Server) {
    this.serverToEdit = { ...server };
    this.editServerPasswordModal.fire();
  }

  editServerName(server: Server) {
    this.serverToEdit = { ...server };
    this.editServerNameModal.fire();
  }

  updateServerRcon() {
    this.isUpdating = true;
    this.serverService.updateServerRconPassword(this.serverToEdit.id, this.serverToEdit.rconPassword).subscribe(() => {
      this.editServerPasswordModal.dismiss();
      this.serverToEdit = null;
      this.serverService.getServers().subscribe((servers) => {
        this.servers = servers;
        this.isUpdating = false;
      });
    });
  }

  updateServerName() {
    this.isUpdating = true;
    this.serverService.updateServer(this.serverToEdit).subscribe(() => {
      this.editServerNameModal.dismiss();
      this.serverToEdit = null;
      this.serverService.getServers().subscribe((servers) => {
        this.servers = servers;
        this.isUpdating = false;
      });
    });
  }

}
