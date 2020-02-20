import { Component, OnInit } from '@angular/core';
import { Player } from '../shared/model/player.model';
import { PlayerService } from '../shared/services/player.service';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'app-player-profile',
    templateUrl: './player-profile.component.html',
    styleUrls: ['./player-profile.component.scss']
})
export class PlayerProfileComponent implements OnInit {

    player: Player;

    constructor(private playerService: PlayerService, private route: ActivatedRoute) {

    }

    ngOnInit() {
        this.route.paramMap.pipe().subscribe(params => {
            this.playerService.getPlayer(+params.get('id')).subscribe(response => {
                this.player = response;
            });
        });
    }

}
