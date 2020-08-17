import { Component, OnInit, Input } from '@angular/core';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { TeamService } from '@pages/hub/shared/services/team.service';

@Component({
    selector: 'app-tournament-standings-round-robin-knockout',
    templateUrl: './tournament-standings-round-robin-knockout.component.html'
})
export class TournamentStandingsRoundRobinKnockoutComponent implements OnInit {

    @Input() tournament: Tournament;

    constructor(private teamService: TeamService) { }

    ngOnInit() {

    }

    getLastQualificationSpot() {
        const maxGroupSize = this.tournament.tournamentStages[0].tournamentGroups[0].tournamentGroupTeams.length;
        switch (maxGroupSize) {
            case 3:
                return 2;
            case 4:
                return 2;
            case 5:
                return 3;
            case 6:
                return 4;
            case 7:
                return 4;
            case 8:
                return 5;
        }
        return -1;
    }
}
