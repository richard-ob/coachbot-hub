import { Component, OnInit, Input } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../../shared/services/player.service';
import { PlayerStatistics } from '../../../shared/model/player-statistics.model';
import { PlayerStatisticFilters } from '../../../shared/model/dtos/paged-player-statistics-request-dto.model';

@Component({
    selector: 'app-team-profile-statistics-leaderboard',
    templateUrl: './team-profile-statistics-leaderboard.component.html',
    styleUrls: ['./team-profile-statistics-leaderboard.component.scss']
})
export class TeamProfileStatisticsLeaderboardComponent implements OnInit {
    @Input() teamId: number;
    @Input() statisticSortColumn: string;
    @Input() statisticProperty: string;
    @Input() statisticDisplayName: string;
    playerStatistics: PlayerStatistics[];
    isLoading = true;

    constructor(private route: ActivatedRoute, private playerService: PlayerService) { }

    ngOnInit() {
        this.isLoading = true;
        const filters = new PlayerStatisticFilters();
        filters.teamId = this.teamId;
        this.playerService.getPlayerStatistics(1, undefined, this.statisticSortColumn, 'DESC', filters).subscribe(response => {
            this.playerStatistics = response.items;
            this.isLoading = false;
        });
    }

}
