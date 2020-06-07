import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PlayerService } from '../../shared/services/player.service';
import { PlayerStatisticFilters } from '../../shared/model/dtos/paged-player-statistics-request-dto.model';
import { TimePeriod } from '../../shared/model/time-period.enum';
import { map } from 'rxjs/operators';
import { PlayerStatistics } from '../../shared/model/player-statistics.model';
import { OverviewType } from 'angular2-calendar-heatmap';

@Component({
    selector: 'app-player-profile-statistics',
    templateUrl: './player-profile-statistics.component.html',
    styleUrls: ['./player-profile-statistics.component.scss']
})
export class PlayerProfileStatisticsComponent implements OnInit {

    playerId: number;
    playerStatistics: PlayerStatistics;
    isLoading = true;
    overview = OverviewType.year;
    data = [
        {
            date: "2020-01-01",
            total: 17164,
            details: [{
                name: "Project 1",
                date: "2016-01-01 12:30:45",
                value: 9192
            }, {
                name: "Project 2",
                date: "2016-01-01 13:37:00",
                value: 6753
            },
            {
                name: "Project N",
                date: "2016-01-01 17:52:41",
                value: 1219
            },
            ]
        },
        {
            date: "2020-08-01",
            total: 17164,
            details: [
                {
                    name: "Project 1",
                    date: "2016-08-01 12:30:45",
                    value: 9192
                }, {
                    name: "Project 2",
                    date: "2016-08-01 13:37:00",
                    value: 6753
                },
                {
                    name: "Project N",
                    date: "2016-08-01 17:52:41",
                    value: 1219
                }
            ]
        }];

    constructor(private route: ActivatedRoute, private playerService: PlayerService) { }

    ngOnInit() {
        this.route.parent.paramMap.pipe().subscribe(params => {
            this.playerId = +params.get('id');
            this.loadPlayerStatistics();
        });
    }

    loadPlayerStatistics() {
        this.isLoading = true;
        const filters: PlayerStatisticFilters = {
            playerId: this.playerId,
            timePeriod: TimePeriod.AllTime,
            includeSubstituteAppearances: true
        };
        this.playerService.getPlayerStatistics(1, undefined, undefined, undefined, filters)
            .pipe(map(result => result.items[0])).subscribe(playerStatistics => {
                this.playerStatistics = playerStatistics;
                this.isLoading = false;
            });
    }
}
