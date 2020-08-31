import { Component, OnInit, Input } from '@angular/core';
import { Router } from '@angular/router';
import { MatchService } from '@pages/hub/shared/services/match.service';
import { PlayerOfTheMatchStatistics } from '@pages/hub/shared/model/player-of-the-match-statistics.model';
import { PositionGroup } from '@pages/hub/shared/model/position-group.enum';

@Component({
    selector: 'app-player-of-the-match',
    templateUrl: './player-of-the-match.component.html'
})
export class PlayerOfTheMatchComponent implements OnInit {

    @Input() matchId: number;
    playerOfTheMatchStatistics: PlayerOfTheMatchStatistics;
    positionGroups = PositionGroup;
    showGoals = false;
    showAssists = false;
    showConceded = false;
    showInterceptions = false;
    showPassCompletion = false;
    showKeeperSaves = false;
    isLoading = true;

    constructor(private router: Router, private matchService: MatchService) { }

    ngOnInit() {
        this.matchService.getPlayerOfTheMatchStatistics(this.matchId).subscribe(statistics => {
            this.playerOfTheMatchStatistics = statistics;
            this.determineStatsToShow();
            this.isLoading = false;
        });
    }

    navigateToPlayer() {
        this.router.navigate(['/player-profile/', this.playerOfTheMatchStatistics.playerId]);
    }

    determineStatsToShow() {
        switch (this.playerOfTheMatchStatistics.positionGroup) {
            case PositionGroup.Goalkeeper:
                this.showKeeperSaves = true;
                this.showConceded = true;
                if (this.playerOfTheMatchStatistics.assists) {
                    this.showAssists = true;
                } else {
                    this.showPassCompletion = true;
                }
                break;
            case PositionGroup.Defence:
                this.showInterceptions = true;
                this.showConceded = true;
                if (this.playerOfTheMatchStatistics.goals) {
                    this.showAssists = true;
                } else if (this.playerOfTheMatchStatistics.assists) {
                    this.showAssists = true;
                } else {
                    this.showPassCompletion = true;
                }
                break;
            case PositionGroup.Midfield:
                this.showAssists = true;
                this.showGoals = true;
                this.showPassCompletion = true;
                break;
            case PositionGroup.Attack:
                this.showAssists = true;
                this.showGoals = true;
                this.showPassCompletion = true;
                break;
        }
    }

}
