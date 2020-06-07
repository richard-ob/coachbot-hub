import { Component, OnChanges, Input, EventEmitter, Output, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerStatisticFilters } from '@pages/hub/shared/model/dtos/paged-player-statistics-request-dto.model';
import { FantasyPlayer } from '@pages/hub/shared/model/fantasy-player.model';
import { TournamentService } from '@pages/hub/shared/services/tournament.service';
import { Team } from '@pages/hub/shared/model/team.model';
import { PositionGroup } from '@pages/hub/shared/model/position-group.enum';
import { FantasyService } from '@pages/hub/shared/services/fantasy.service';

@Component({
    selector: 'app-fantasy-team-editor-players',
    templateUrl: './fantasy-team-editor-players.component.html',
    styleUrls: ['./fantasy-team-editor-players.component.scss']
})
export class FantasyTeamEditorPlayersComponent implements OnChanges, OnInit {

    @Input() tournamentEditionId: number;
    @Output() playerSelected = new EventEmitter<FantasyPlayer>();
    filters: PlayerStatisticFilters = new PlayerStatisticFilters();
    fantasyPlayers: FantasyPlayer[];
    teams: Team[];
    ratingRange: number[] = [0, 10];
    positionGroups = PositionGroup;
    currentPage = 1;
    totalPages: number;
    totalItems: number;
    sortBy: string = null;
    sortOrder = 'ASC';
    isLoading = false;
    isSaving = true;

    constructor(private fantasyService: FantasyService, private tournamentService: TournamentService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.tournamentService.getTournamentEdition(this.tournamentEditionId).subscribe(tournamentEdition => {
            this.tournamentService.getTournamentTeams(tournamentEdition.id).subscribe(teams => {
                this.teams = teams;
            });
        });
    }

    ngOnChanges() {
        this.filters.tournamentEditionId = this.tournamentEditionId;
        this.loadFantasyPlayers(this.currentPage);
    }

    loadFantasyPlayers(page: number, sortBy: string = null) {
        this.isLoading = true;
        if (sortBy !== null && this.sortBy !== null && this.sortBy === sortBy && this.sortOrder === 'ASC') {
            this.sortOrder = 'DESC';
        } else {
            this.sortOrder = 'ASC';
        }
        this.sortBy = sortBy || this.sortBy;
        this.isLoading = true;
        this.fantasyService.getFantasyPlayers(page, undefined, this.sortBy, this.sortOrder, this.filters).subscribe(response => {
            this.fantasyPlayers = response.items;
            this.currentPage = response.page;
            this.totalPages = response.totalPages;
            this.totalItems = response.totalItems;
            this.isLoading = false;
        });
    }

    selectFantasyPlayer(fantasyPlayer: FantasyPlayer) {
        this.playerSelected.emit(fantasyPlayer);
    }

    setRatingRange() {
        this.filters.minimumRating = this.ratingRange[0];
        this.filters.maximumRating = this.ratingRange[1];
    }

}
