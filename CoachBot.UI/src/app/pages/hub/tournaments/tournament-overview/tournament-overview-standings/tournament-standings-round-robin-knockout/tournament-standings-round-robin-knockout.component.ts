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
}
