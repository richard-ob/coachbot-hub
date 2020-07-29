import { Component, OnInit } from '@angular/core';
import { EventType } from './model/match-data.interface';
import { StatisticType } from './model/statistic-type.enum';
import { TeamType } from './model/team-type.enum.js';
import { DisplayValueMode } from './components/horizontal-bar-graph/horizontal-bar-graph.component.js';
import { MatchService } from '../shared/services/match.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Match } from '../shared/model/match.model';
import { MatchTeamType } from '../shared/model/match-team-type.enum';
import { PlayerService } from '../shared/services/player.service';
import { Player } from '../shared/model/player.model';
import { MatchStatisticsService } from '../shared/services/match-statistics.service';
import { PlayerHubRole } from '../shared/model/player-hub-role.enum';

@Component({
  selector: 'app-match-overview',
  templateUrl: './match-overview.component.html',
  styleUrls: ['./match-overview.component.scss']
})
export class MatchOverviewComponent implements OnInit {
  // TODO: ADD SUBSTITUTES TO EVENTS LIST
  matchData: any;
  match: Match;
  statisticType = StatisticType;
  teamType = TeamType;
  matchTeamType = MatchTeamType;
  eventType = EventType;
  displayValueModes = DisplayValueMode;
  currentPlayer: Player;
  hubRoles = PlayerHubRole;
  matchDate: Date;
  isLoading = true;

  constructor(
    private matchService: MatchService,
    private matchStatisticsService: MatchStatisticsService,
    private route: ActivatedRoute,
    private playerService: PlayerService,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.paramMap.pipe().subscribe(params => {
      this.matchService.getMatch(+params.get('id')).subscribe(match => {
        this.match = match;
        this.isLoading = false;
        if (match.matchStatistics) {
          this.loadJson(match.matchStatistics.matchData);
        }
      });
    });
    this.playerService.getCurrentPlayer().subscribe(player => this.currentPlayer = player);
  }

  loadJson(matchData: any) {
    this.matchData = matchData;
    this.matchDate = new Date(this.matchData.matchInfo.startTime * 1000);
  }

  navigateToTeamProfile(teamId: number) {
    this.router.navigate(['/team-profile', teamId]);
  }

  swapTeams() {
    this.isLoading = true;
    this.matchStatisticsService.swapTeams(this.match.matchStatistics.id).subscribe(() => {
      this.isLoading = false;
      location.reload();
    });
  }
}
