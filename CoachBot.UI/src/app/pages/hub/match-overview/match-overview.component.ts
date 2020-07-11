import { Component, OnInit } from '@angular/core';
import { EventType } from './model/match-data.interface';
import { StatisticType } from './model/statistic-type.enum';
import { TeamType } from './model/team-type.enum.js';
import { DisplayValueMode } from './components/horizontal-bar-graph/horizontal-bar-graph.component.js';
import { MatchService } from '../shared/services/match.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Match } from '../shared/model/match.model';
import { MatchTeamType } from '../shared/model/match-team-type.enum';

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
  matchDate: Date;
  isLoading = true;

  constructor(
    private matchService: MatchService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.paramMap.pipe().subscribe(params => {
      this.matchService.getMatch(+params.get('id')).subscribe(match => {
        this.match = match;
        this.loadJson(match.matchStatistics.matchData);
        this.isLoading = false;
      });
    });
  }

  loadJson(matchData: any) {
    this.matchData = matchData;
    this.matchDate = new Date(this.matchData.matchInfo.startTime * 1000);
  }

  navigateToTeamProfile(teamId: number) {
    this.router.navigate(['/team-profile', teamId]);
  }
}
