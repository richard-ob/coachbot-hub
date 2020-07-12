import { Component, OnInit, Input } from '@angular/core';
import { PlayerPositionMatchStatistics } from '@pages/hub/shared/model/player-position-match-statistics.model';
import ColourUtils from '@shared/utilities/colour-utilities';

@Component({
    selector: 'app-match-overview-lineup-position',
    templateUrl: './match-overview-lineup-position.component.html',
    styleUrls: ['./match-overview-lineup-position.component.scss']
})
export class MatchOverviewLineupPositionComponent implements OnInit {

    @Input() color: string;
    @Input() player: PlayerPositionMatchStatistics;
    @Input() substitutes: PlayerPositionMatchStatistics[];
    @Input() substituted = false;
    mainSub: PlayerPositionMatchStatistics;
    svgKitColor: string;
    fontColor: string;
    isLoading = true;

    constructor() { }

    ngOnInit() {
        this.fontColor = this.getFontColour();
        this.mainSub = this.getMainSub();
        console.log(this.substitutes);
    }

    getFontColour() {
        return ColourUtils.isDark(this.color) ? '#000000' : '#ffffff';
    }

    getMainSub() {
        return this.substitutes.sort((a, b) => a.distanceCovered - b.distanceCovered).find(p => p.playerId !== this.player.playerId);
    }
}
