import { Component, OnInit } from '@angular/core';
import sampleMatchDataJson from './sample-data/sample.json';
import { MatchData } from './model/match-data.interface';
import { StatisticType } from './model/statistic-type.enum';
import { TeamType } from './model/team-type.enum.js';

@Component({
  selector: 'app-match-overview',
  templateUrl: './match-overview.component.html',
  styleUrls: ['./match-overview.component.scss']
})
export class MatchOverviewComponent implements OnInit {

  sampleMatchData: any = sampleMatchDataJson;
  statisticType = StatisticType;
  teamType = TeamType;

  // Match Data
  matchDate: Date = new Date(this.sampleMatchData.matchInfo.startTime * 1000);

  constructor() { }

  ngOnInit() {
  }

}
