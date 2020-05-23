import { Component, OnInit } from '@angular/core';
import { EventType } from './model/match-data.interface';
import { StatisticType } from './model/statistic-type.enum';
import { TeamType } from './model/team-type.enum.js';
import { DisplayValueMode } from './components/horizontal-bar-graph/horizontal-bar-graph.component.js';
import { MatchService } from '../shared/services/match.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-match-overview',
  templateUrl: './match-overview.component.html',
  styleUrls: ['./match-overview.component.scss']
})
export class MatchOverviewComponent implements OnInit {

  matchData: any;
  statisticType = StatisticType;
  teamType = TeamType;
  eventType = EventType;
  displayValueModes = DisplayValueMode;
  matchDate: Date;
  isLoading = true;

  constructor(private matchService: MatchService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.paramMap.pipe().subscribe(params => {
      this.matchService.getMatch(+params.get('id')).subscribe(response => {
        this.loadJson(response.matchStatistics.matchData);
        this.isLoading = false;
      });
    });
  }

  loadJson(matchData: any) {
    this.matchData = matchData;
    this.matchDate = new Date(this.matchData.matchInfo.startTime * 1000);
  }
}
