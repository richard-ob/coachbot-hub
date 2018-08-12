import { Component } from '@angular/core';
import { MatchService } from '../shared/services/match.service';
import { UserService } from '../shared/services/user.service';
import { Match } from '../model/match';
import { User } from '../model/user';
import { Channel } from '../model/channel';
import { Player } from '../model/player';

@Component({
    selector: 'app-match-history',
    templateUrl: './match-history.component.html'
})
export class MatchHistoryComponent {

    matchHistory: Match[];
    user: User;
    currentPage = 1;
    currentChannel: Channel;

    constructor(private matchService: MatchService, private userService: UserService) {
        this.userService.getUser().subscribe(user => {
            this.user = user;
            if (this.user.channels.length > 0) {
                this.currentChannel = this.user.channels[0];
                this.matchService.getMatchHistory(this.currentChannel.idString)
                    .subscribe(matchHistory => this.matchHistory = matchHistory);
            }
        });
    }

    loadMatchHistory() {
        this.matchService.getMatchHistory(this.currentChannel.idString)
            .subscribe(matchHistory => this.matchHistory = matchHistory);
    }

    generatePlayerList(players: Player[]) {
        let playerList = '';
        for (const player of players) {
            if (players[0].name === player.name) {
                playerList = `${playerList} ${player.name}`;
            } else {
                playerList = `${playerList}, ${player.name}`;
            }
        }
        return playerList;
    }

    cleanName(teamName: string) {
        if (teamName.indexOf('>') > 0) {
            teamName = teamName.substring(2);
            teamName = teamName.substring(0, teamName.indexOf(':'));
        }
        return teamName;
    }
}
