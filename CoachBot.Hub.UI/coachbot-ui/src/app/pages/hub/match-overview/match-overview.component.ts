import { Component, OnInit } from '@angular/core';
import sampleMatchDataJson from './sample-data/sample.json';
import { MatchData, EventType } from './model/match-data.interface';
import { StatisticType } from './model/statistic-type.enum';
import { TeamType } from './model/team-type.enum.js';
import { DisplayValueMode } from './components/horizontal-bar-graph/horizontal-bar-graph.component.js';
import { MatchService } from '../shared/services/match.service';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-match-overview',
  templateUrl: './match-overview.component.html',
  styleUrls: ['./match-overview.component.scss']
})
export class MatchOverviewComponent implements OnInit {

  sampleMatchData: any;
  statisticType = StatisticType;
  teamType = TeamType;
  eventType = EventType;
  displayValueModes = DisplayValueMode;
  matchDate: Date;

  constructor(private matchService: MatchService, private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.paramMap.pipe().subscribe(params => {
      this.matchService.getMatch(+params.get('id')).subscribe(response => {
        this.loadJson(response.matchStatistics.matchData);
      });
    });
  }

  loadJson(matchData: any) {
    this.sampleMatchData = matchData;
    this.matchDate = new Date(this.sampleMatchData.matchInfo.startTime * 1000);
  }
}
