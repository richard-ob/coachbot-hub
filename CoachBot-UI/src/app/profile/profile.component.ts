import { Component } from '@angular/core';
import { UserService } from '../shared/services/user.service';
import { User } from '../model/user';
import { LeaderboardService } from '../shared/services/leaderboard.service';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html'
})
export class ProfileComponent {

    user: User;
    channelLeaderboard: any[];
    userStats: any;

    constructor(private leaderboardService: LeaderboardService, private userService: UserService) {
        this.userService.getUser().subscribe(user => {
            this.user = user;
            this.leaderboardService.getLeaderboardForPlayer(this.user.discordUserIdString)
                .subscribe(channelLeaderboard => this.channelLeaderboard = channelLeaderboard);
        });
        this.userService.getUserStatistics().subscribe(userStats => this.userStats = userStats);
    }
}
