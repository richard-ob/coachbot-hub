import { Component, OnInit, Input } from '@angular/core';
import { Tournament } from '@pages/hub/shared/model/tournament.model';
import { TournamentGroupMatch } from '@pages/hub/shared/model/tournament-group-match.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { TournamentGroupStanding } from '@pages/hub/shared/model/tournament-group-standing.model';
import { Router } from '@angular/router';
import { map } from 'rxjs/operators';
import { MatchOutcomeType } from '@pages/hub/shared/model/match-outcome-type.enum';

@Component({
    selector: 'app-tournament-standings-round-robin',
    templateUrl: './tournament-standings-round-robin.component.html',
    styleUrls: ['./tournament-standings-round-robin.component.scss']
})
export class TournamentStandingsRoundRobinComponent implements OnInit {

    @Input() tournament: Tournament;
    @Input() tournamentGroupId: number;
    @Input() tournamentGroupName: string;
    @Input() lastQualificationSpot: number;
    @Input() showFullHeaders = false;
    tournamentGroupStandings: TournamentGroupStanding[];
    isLoading = true;
    matches: TournamentGroupMatch;

    constructor(private tournamentService: TournamentService, private router: Router) { }

    ngOnInit() {
        this.tournamentService.getTournamentGroupStandings(
            this.tournamentGroupId || this.tournament.tournamentStages[0].tournamentGroups[0].id
        )
            .pipe(
                map(groupStandings => {
                    groupStandings.forEach(standing => {
                        standing.form = this.padArray(standing.form, 5, MatchOutcomeType.Unknown);
                    });
                    return groupStandings;
                })
            )
            .subscribe(standings => {
                this.tournamentGroupStandings = standings;
                this.isLoading = false;
            });
    }

    navigateToTeamProfile(teamId: number) {
        this.router.navigate(['/team-profile/', teamId]);
    }

    padArray(array: any[], length: number, fill: any) {
        return length > array.length ? array.concat(Array(length - array.length).fill(fill)) : array;
    }
}
