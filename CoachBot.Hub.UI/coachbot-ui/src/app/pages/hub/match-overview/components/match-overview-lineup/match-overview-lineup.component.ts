import { Component, OnInit, Input } from '@angular/core';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { PlayerStatisticFilters } from '@pages/hub/shared/model/dtos/paged-player-statistics-request-dto.model';
import { PlayerPositionMatchStatistics } from '@pages/hub/shared/model/player-match-statistics.model';
import { Positions } from './positions';


@Component({
    selector: 'app-match-overview-lineup',
    templateUrl: './match-overview-lineup.component.html',
    styleUrls: ['./match-overview-lineup.component.scss']
})
export class MatchOverviewLineupComponent implements OnInit {

    @Input() matchId: number;
    @Input() channelId: number;
    filters = new PlayerStatisticFilters();
    players: PlayerPositionMatchStatistics[];
    starters: PlayerPositionMatchStatistics[];
    defPositions = Positions.DEF;
    midPositions = Positions.MID;
    atkPositions = Positions.ATK;
    isLoading = true;

    constructor(private playerService: PlayerService) { }

    ngOnInit() {
        this.filters.matchId = this.matchId;
        this.filters.channelId = this.channelId;
        this.playerService.getPlayerMatchStatistics(1, undefined, undefined, undefined, this.filters).subscribe(players => {
            this.players = players.items;
            this.starters = this.players.filter(p => p.substitute); // TODO: invert this.. the original statistic generation was wrong
            this.isLoading = false;
        });
    }

}
