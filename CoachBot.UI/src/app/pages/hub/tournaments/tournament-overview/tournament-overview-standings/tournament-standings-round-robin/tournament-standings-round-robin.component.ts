import { Component, OnInit, Input } from '@angular/core';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { TournamentGroupMatch } from '@pages/hub/shared/model/tournament-group-match.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { TournamentGroupStanding } from '@pages/hub/shared/model/tournament-group-standing.model';
import { Router } from '@angular/router';

@Component({
    selector: 'app-tournament-standings-round-robin',
    templateUrl: './tournament-standings-round-robin.component.html'
})
export class TournamentStandingsRoundRobinComponent implements OnInit {

    @Input() tournament: Tournament;
    @Input() tournamentGroupId: number;
    @Input() tournamentGroupName: string;
    tournamentGroupStandings: TournamentGroupStanding[];
    isLoading = true;
    matches: TournamentGroupMatch;

    constructor(private tournamentService: TournamentService, private router: Router) { }

    ngOnInit() {
        this.tournamentService.getTournamentGroupStandings(this.tournamentGroupId).subscribe(standings => {
            this.tournamentGroupStandings = standings;
            this.isLoading = false;
        });
    }

    navigateToTeamProfile(teamId: number) {
        this.router.navigate(['/team-profile/', teamId]);
    }
}
