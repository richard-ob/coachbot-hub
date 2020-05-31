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

  @ViewChild('editServerModal', { static: false }) editServerModal: SwalComponent;
  newServer: Server = new Server();
  servers: Server[];
  regions: Region[];
  countries: Country[];
  serverToEdit: Server;
  isLoading = true;
  isAdding = false;
  isRemoving = false;

  constructor(
    private serverService: ServerService,
    private regionService: RegionService,
    private countryService: CountryService,
    public readonly swalTargets: SwalPortalTargets
  ) {
    forkJoin(
      this.serverService.getServers(),
      this.regionService.getRegions(),
      this.countryService.getCountries()
    ).subscribe(([servers, regions, countries]) => {
      this.servers = servers;
      this.regions = regions;
      this.countries = countries;
      this.isLoading = false;
    });
  }

  addServer() {
    this.isAdding = true;
    this.serverService.addServer(this.newServer).subscribe(() => {
      this.serverService.getServers().subscribe(servers => this.servers = servers);
      this.newServer = new Server();
      this.isAdding = false;
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

  editServer(server: Server) {
    this.serverToEdit = { ...server };
    this.editServerModal.fire();
  }

  updateServer() {
    this.serverService.updateServer(this.serverToEdit).subscribe(() => {
      this.editServerModal.dismiss();
      this.serverToEdit = null;
      this.serverService.getServers().subscribe((servers) => {
        this.servers = servers;
      });
    });
  }

}
