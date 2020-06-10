import { Component, OnInit, Input } from '@angular/core';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { TournamentEdition } from '@pages/hub/shared/model/tournament-edition.model';
import { TournamentGroupMatch } from '@pages/hub/shared/model/tournament-group-match.model';

@Component({
    selector: 'app-tournament-standings-knockout',
    templateUrl: './tournament-standings-knockout.component.html'
})
export class TournamentStandingsKnockoutComponent implements OnInit {

    @Input() tournamentEditionId: number;
    tournamentEdition: TournamentEdition;
    matches: TournamentGroupMatch;
    isLoading = true;

    constructor(private tournamentService: TournamentService) {
    }

    ngOnInit() {
        this.loadTournamentEdition();
    }

    loadTournamentEdition() {
        this.isLoading = true;
        this.tournamentService.getTournamentEdition(this.tournamentEditionId).subscribe(tournamentEdition => {
            this.tournamentEdition = tournamentEdition;
            this.isLoading = false;
        });
    }


}
