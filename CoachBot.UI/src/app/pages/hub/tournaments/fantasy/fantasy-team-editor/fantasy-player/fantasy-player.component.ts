import { Component, Input, OnInit } from '@angular/core';
import { FantasyPlayer } from '@pages/hub/shared/model/fantasy-player.model';
import { PositionGroup } from '@pages/hub/shared/model/position-group.enum';
import { cssColouriser } from './css-colouriser.utility';

@Component({
    selector: 'app-fantasy-player',
    templateUrl: './fantasy-player.component.html',
    styleUrls: ['./fantasy-player.component.scss']
})
export class FantasyPlayerComponent implements OnInit {

    @Input() fantasyPlayer: FantasyPlayer;
    @Input() viewMode = false;
    @Input() isFlex = false;
    playerName: string;
    kitImageUrl: string;
    rating: number;
    badge: string;
    positionGroup: PositionGroup;

    constructor() { }

    ngOnInit(): void {
        this.rating = this.fantasyPlayer.rating;
        this.positionGroup = this.fantasyPlayer.positionGroup;
        this.playerName = this.fantasyPlayer.player.name;
        this.kitImageUrl = 'assets/images/iosoccer/fantasy/kits/' + this.fantasyPlayer.team.teamCode.toLowerCase() + '_orig.png';
        if (this.fantasyPlayer.team.badgeImage) {
            this.badge = this.fantasyPlayer.team.badgeImage.extraSmallUrl;
        }
    }

}
