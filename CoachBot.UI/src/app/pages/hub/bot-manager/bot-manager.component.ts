import { Component, OnInit } from '@angular/core';
import { BotService } from '../shared/services/bot.service';
import { PlayerService } from '../shared/services/player.service';
import { Player } from '../shared/model/player.model';
import { PlayerHubRole } from '../shared/model/player-hub-role.enum';

@Component({
    selector: 'app-bot-manager',
    templateUrl: './bot-manager.component.html'
})
export class BotManagerComponent implements OnInit {

    currentPlayer: Player;
    playerHubRoles = PlayerHubRole;

    constructor(private botService: BotService, private playerService: PlayerService) { }

    ngOnInit() {
        this.playerService.getCurrentPlayer().subscribe(currentPlayer => this.currentPlayer = currentPlayer);
    }

}
