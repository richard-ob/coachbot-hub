import { Component } from '@angular/core';
import { UserService } from '../shared/services/user.service';
import { User } from '../model/user';
import { LeaderboardService } from '../shared/services/leaderboard.service';
import { Player } from '../model/player';
import { Channel } from '../model/channel';

@Component({
    selector: 'app-player-leaderboards',
    templateUrl: './player-leaderboards.component.html'
})
export class PlayerLeaderboardsComponent {

    user: User;
    playerLeaderboard: any[];
    channelLeaderboard: any[];
    currentChannel: Channel;

    constructor(private leaderboardService: LeaderboardService, private userService: UserService) {
        this.userService.getUser().subscribe(user => {
            this.user = user;
            this.leaderboardService.getPlayerLeaderboard().subscribe(playerLeaderboard => {
                this.playerLeaderboard = playerLeaderboard;
                let i = 1;
                for (const player of this.playerLeaderboard) {
                    player.rank = i;
                    i++;
                }
            });
            if (this.user.channels.length > 0) {
                this.currentChannel = this.user.channels[0];
                this.leaderboardService.getPlayerLeaderboardForChannel(this.currentChannel.idString)
                    .subscribe(channelLeaderboard => {
                        this.channelLeaderboard = channelLeaderboard;
                        let i = 1;
                        for (const channel of this.channelLeaderboard) {
                            channel.rank = i;
                            i++;
                        }
                    });
            }
        });
    }

    loadChannelLeaderboard() {
        this.channelLeaderboard = null;
        this.leaderboardService.getPlayerLeaderboardForChannel(this.currentChannel.idString)
            .subscribe(channelLeaderboard => {
                this.channelLeaderboard = channelLeaderboard;
                let i = 1;
                for (const channel of this.channelLeaderboard) {
                    channel.rank = i;
                    i++;
                }
            });
    }

}
