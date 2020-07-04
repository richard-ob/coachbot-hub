import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TournamentService } from '../../../shared/services/tournament.service';
import { TournamentStaff } from '@pages/hub/shared/model/tournament-staff.model';

@Component({
    selector: 'app-tournament-overview-staff',
    templateUrl: './tournament-overview-staff.component.html'
})
export class TournamentOverviewStaffComponent implements OnInit {

    tournamentId: number;
    tournamentStaff: TournamentStaff[];
    isLoading = true;
    isSaving = false;

    constructor(private route: ActivatedRoute, private tournamentService: TournamentService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.loadTournamentStaff();
        });
    }

    loadTournamentStaff() {
        this.isLoading = true;
        this.tournamentService.getTournamentStaff(this.tournamentId).subscribe(tournamentStaff => {
            this.tournamentStaff = tournamentStaff;
            this.isLoading = false;
        });
    }

}
