import { Component, OnInit, Input } from '@angular/core';
import { PlayerService } from '../../../shared/services/player.service';
import { PlayerStatistics } from '../../../shared/model/player-statistics.model';
import { PlayerStatisticFilters } from '../../../shared/model/dtos/paged-player-statistics-request-dto.model';
import { MatchTypes } from '../../model/match-types.enum';

@Component({
    selector: 'app-statistics-leaderboard',
    templateUrl: './statistics-leaderboard.component.html',
    styleUrls: ['./statistics-leaderboard.component.scss']
})
export class StatisticsLeaderboardComponent implements OnInit {
    @Input() teamId: number;
    @Input() oppositionTeamId: number;
    @Input() statisticSortColumn: string;
    @Input() statisticProperty: string;
    @Input() statisticDisplayName: string;
    @Input() matchType: MatchTypes;
    playerStatistics: PlayerStatistics[];
    isLoading = true;

    constructor(private playerService: PlayerService) { }

    ngOnInit() {
        this.isLoading = true;
        const filters = new PlayerStatisticFilters();
        filters.teamId = this.teamId;
        filters.oppositionTeamId = this.oppositionTeamId;
        filters.matchType = this.matchType;
        filters.matchFormat = null;
        this.playerService.getPlayerStatistics(1, undefined, this.statisticSortColumn, 'DESC', filters).subscribe(response => {
            this.playerStatistics = response.items;
            this.isLoading = false;
        });
    }

}
