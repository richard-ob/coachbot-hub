import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';

@Component({
    selector: 'app-tournament-actions',
    templateUrl: './tournament-actions.component.html'
})
export class TournamentActionsComponent implements OnInit {

    tournamentId: number;
    tournament: Tournament;
    isSaving = false;
    isLoading = true;

    constructor(
        private tournamentService: TournamentService,
        private route: ActivatedRoute,
        private fantasyService: FantasyService,
        private router: Router
    ) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.tournamentId = +params.get('id');
            this.isLoading = false;
        });
    }

    generateSchedule() {
        this.isLoading = true;
        this.tournamentService.generateTournamentSchedule(this.tournamentId).subscribe(() => {
            this.isLoading = false;
        });
    }

    generateFantasyTeamSnapshots() {
        this.isLoading = true;
        this.fantasyService.generateFantasySnapshots(this.tournamentId).subscribe(() => {
            this.isLoading = false;
        });
    }


}
