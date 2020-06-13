import { Component, OnInit } from '@angular/core';
import { Organisation } from '@pages/hub/shared/model/organisation.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-organisation-editor',
    templateUrl: './organisation-editor.component.html'
})
export class OrganisationEditorComponent implements OnInit {

    organisationId: number;
    organisation: Organisation;
    isLoading = true;
    isSaving = false;

    constructor(private tournamentService: TournamentService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            console.log('hi');
            console.log(params);
            this.organisationId = +params.get('id');
            this.loadOrganisation();
        });
    }

    loadOrganisation() {
        this.isLoading = true;
        this.tournamentService.getOrganisation(this.organisationId).subscribe(organisation => {
            this.organisation = organisation;
            this.organisation.brandColour = this.organisation.brandColour || '#000000';
            this.isLoading = false;
        });
    }

    updateOrganisation() {
        this.isSaving = true;
        this.tournamentService.updateOrganisation(this.organisation).subscribe(() => {
            this.isSaving = false;
            this.loadOrganisation();
        });
    }
}
