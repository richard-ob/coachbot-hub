import { Component, ViewChild, Output, EventEmitter, DoCheck } from '@angular/core';
import { SwalPortalTargets, SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import { ServerService } from '@pages/hub/shared/services/server.service';
import { Server } from '@pages/hub/shared/model/server.model';
import { forkJoin } from 'rxjs';
import { Region } from '@pages/hub/shared/model/region.model';
import { Country } from '@pages/hub/shared/model/country.model';
import { CountryService } from '@pages/hub/shared/services/country.service';
import { RegionService } from '@pages/hub/shared/services/region.service';
import { NgForm } from '@angular/forms';
@Component({
    selector: 'app-server-creator',
    templateUrl: './server-creator.component.html'
})
export class ServerCreatorComponent implements DoCheck {

    @ViewChild('createServerModal') createServerModal: SwalComponent;
    @ViewChild('createServerForm') createServerForm: NgForm;
    @Output() serverCreated = new EventEmitter<void>();
    serverToCreate: Server;
    servers: Server[];
    regions: Region[];
    countries: Country[];
    isAdding = true;
    isLoading = true;

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

    ngDoCheck() {
        if (this.createServerModal && this.createServerModal.swalVisible) {
            const swalConfirm = document.querySelector('.swal2-confirm') as any;
            if (swalConfirm) {
                swalConfirm.disabled = this.createServerForm.invalid;
            }
        }
    }

    openModal() {
        this.serverToCreate = new Server();
        this.createServerModal.fire();
        // tslint:disable-next-line:no-string-literal
        console.log(this.createServerModal);
    }

    addServer() {
        this.isAdding = true;
        this.serverService.addServer(this.serverToCreate).subscribe(() => {
            this.isAdding = false;
            this.serverCreated.emit();
        });
    }

}
