import { Component, OnInit } from '@angular/core';
import { Server } from '../shared/model/server.model';
import { Region } from '../shared/model/region.model';
import { Country } from '../shared/model/country.model';
import { ServerService } from '../shared/services/server.service';
import { RegionService } from '../shared/services/region.service';
import { CountryService } from '../shared/services/country.service';

@Component({
  selector: 'app-server-manager',
  templateUrl: './server-manager.component.html',
  styleUrls: ['./server-manager.component.scss']
})
export class ServerManagerComponent {

  newServer: Server = new Server();
  servers: Server[];
  regions: Region[];
  countries: Country[];

  constructor(private serverService: ServerService, private regionService: RegionService, private countryService: CountryService) {
    this.serverService.getServers().subscribe(servers => this.servers = servers);
    this.regionService.getRegions().subscribe(regions => this.regions = regions);
    this.countryService.getCountries().subscribe(countries => this.countries = countries);
  }

  addServer() {
    this.serverService.addServer(this.newServer).subscribe(() => {
      this.serverService.getServers().subscribe(servers => this.servers = servers);
      this.newServer = new Server();
    });
  }

  removeServer(serverId: number) {
    this.serverService.removeServer(serverId).subscribe(() => {
      this.serverService.getServers().subscribe(servers => this.servers = servers);
    });
  }

}
