import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TournamentEditionStaff } from '../../../shared/model/tournament-edition-staff.model';
import { TournamentService } from '../../../shared/services/tournament.service';

@Component({
    selector: 'app-tournament-overview-staff',
    templateUrl: './tournament-overview-staff.component.html'
})
export class TournamentOverviewStaffComponent implements OnInit {

    tournamentEditionId: number;
    tournamentEditionStaff: TournamentEditionStaff[];
    isLoading = true;
    isSaving = false;

    constructor(private route: ActivatedRoute, private tournamentService: TournamentService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentEditionId = +params.get('id');
            this.loadTournamentEditionStaff();
        });
    }

    loadTournamentEditionStaff() {
        this.isLoading = true;
        this.tournamentService.getTournamentEditionStaff(this.tournamentEditionId).subscribe(tournamentEditionStaff => {
            this.tournamentEditionStaff = tournamentEditionStaff;
            this.isLoading = false;
        });
    }

}
