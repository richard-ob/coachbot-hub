import { Component, OnInit } from '@angular/core';
import { Organisation } from '@pages/hub/shared/model/organisation.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { Router } from '@angular/router';

@Component({
    selector: 'app-organisations',
    templateUrl: './organisations.component.html'
})
export class OrganisationsComponent implements OnInit {

    organisations: Organisation[];
    organisation: Organisation = new Organisation();
    isLoading = true;

    constructor(private tournamentService: TournamentService, private router: Router) { }

    ngOnInit() {
        this.loadOrganisations();
    }

    loadOrganisations() {
        this.isLoading = true;
        this.tournamentService.getOrganisations().subscribe(organisations => {
            this.organisations = organisations;
            this.isLoading = false;
        });
    }

    updateLogoImageId(assetImageId: number) {
        this.organisation.logoImageId = assetImageId;
    }

    createOrganisation() {
        this.isLoading = true;
        this.tournamentService.createOrganisation(this.organisation).subscribe(() => {
            this.isLoading = false;
            this.organisation = new Organisation();
            this.loadOrganisations();
        });
    }

    editOrganisation(organisationId: number) {
        this.router.navigate(['/organisation-editor/' + organisationId]);
    }
}
