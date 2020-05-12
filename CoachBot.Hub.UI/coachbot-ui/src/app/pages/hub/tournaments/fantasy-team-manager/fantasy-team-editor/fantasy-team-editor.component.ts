import { Component, OnInit } from '@angular/core';
import { FantasyService } from '../../../shared/services/fantasy.service';
import { FantasyTeam } from '../../../shared/model/fantasy-team.model';
import { ActivatedRoute } from '@angular/router';
import { FantasyPlayer } from '../../../shared/model/fantasy-player.model';
import { PlayerStatisticFilters } from '../../../shared/model/dtos/paged-player-statistics-request-dto.model';
import { TournamentService } from '../../../shared/services/tournament.service';
import { TournamentEdition } from '../../../shared/model/tournament-edition.model';
import { Team } from '../../../shared/model/team.model';
import { FantasyTeamSelection } from '../../../shared/model/fantasy-team-selection.model';
import { PositionGroup } from '../../../shared/model/position-group.enum';

@Component({
    selector: 'app-fantasy-team-editor',
    templateUrl: './fantasy-team-editor.component.html',
    styleUrls: ['./fantasy-team-editor.component.scss']
})
export class FantasyTeamEditorComponent implements OnInit {

    fantasyTeamId: number;
    fantasyTeam: FantasyTeam;
    fantasyPlayers: FantasyPlayer[];
    filters: PlayerStatisticFilters = new PlayerStatisticFilters();
    tournamentEdition: TournamentEdition;
    teams: Team[];
    ratingRange: number[] = [0, 10];
    positionGroups = PositionGroup;
    isLoading = false;
    isSaving = true;

    constructor(private fantasyService: FantasyService, private tournamentService: TournamentService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.fantasyTeamId = +params.get('id');
            this.fantasyService.getFantasyTeam(this.fantasyTeamId).subscribe(fantasyTeam => {
                this.fantasyTeam = fantasyTeam;
                this.tournamentService.getTournamentEdition(this.fantasyTeam.tournamentEditionId).subscribe(tournamentEdition => {
                    this.tournamentEdition = tournamentEdition;
                    this.tournamentService.getTournamentTeams(this.tournamentEdition.id).subscribe(teams => {
                        this.teams = teams;
                        this.fantasyService.getFantasyPlayers(this.fantasyTeam.tournamentEditionId).subscribe(fantasyPlayers => {
                            this.fantasyPlayers = fantasyPlayers;
                            this.isLoading = false;
                        });
                    });
                });
            });
        });
    }

    setRatingRange() {
        this.filters.minimumRating = this.ratingRange[0];
        this.filters.maximumRating = this.ratingRange[1];
        console.log(this.filters.minimumRating);
        console.log(this.filters.maximumRating);
    }

    addFantasyTeamSelection(fantasyPlayer: FantasyPlayer) {
        const selection = new FantasyTeamSelection();
        selection.fantasyPlayer = fantasyPlayer;
        selection.fantasyTeamId = this.fantasyTeamId;
        selection.isFlex = false;
        this.fantasyTeam.fantasyTeamSelections.push(selection);
        console.log(this.fantasyTeam);
    }

    removeFantasyTeamSelection() {

    }

}
