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
    @Input() color: string;
    filters = new PlayerStatisticFilters();
    players: PlayerPositionMatchStatistics[];
    starters: PlayerPositionMatchStatistics[];
    substitutes: PlayerPositionMatchStatistics[];
    defPositions = Positions.DEF;
    midPositions = Positions.MID;
    atkPositions = Positions.ATK;
    allPositions = [...Positions.GK, ...this.defPositions, ...this.midPositions, ...this.atkPositions];
    isLoading = true;

    constructor(private playerService: PlayerService) { }

    ngOnInit() {
        this.filters.matchId = this.matchId;
        this.filters.channelId = this.channelId;
        this.playerService.getPlayerPositionMatchStatistics(1, 30, undefined, undefined, this.filters).subscribe(players => {
            this.players = players.items;
            this.starters = this.players
                .filter(p => !p.substitute)
                .sort((a, b) => this.allPositions.indexOf(a.position.name) - this.allPositions.indexOf(b.position.name));
            this.substitutes = this.players
                .filter(p => p.substitute)
                .sort((a, b) => this.allPositions.indexOf(a.position.name) - this.allPositions.indexOf(b.position.name));
            this.isLoading = false;
        });
    }

}
