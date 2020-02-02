import { Component, OnInit } from '@angular/core';
import sampleMatchDataJson from './sample-data/sample.json';
import { MatchData, EventType } from './model/match-data.interface';
import { StatisticType } from './model/statistic-type.enum';
import { TeamType } from './model/team-type.enum.js';
import { DisplayValueMode } from './components/horizontal-bar-graph/horizontal-bar-graph.component.js';

@Component({
  selector: 'app-match-overview',
  templateUrl: './match-overview.component.html',
  styleUrls: ['./match-overview.component.scss']
})
export class MatchOverviewComponent implements OnInit {

  sampleMatchData: any = sampleMatchDataJson;
  statisticType = StatisticType;
  teamType = TeamType;
  eventType = EventType;
  displayValueModes = DisplayValueMode;

  // Match Data
  matchDate: Date = new Date(this.sampleMatchData.matchInfo.startTime * 1000);

  constructor() { }

  ngOnInit() {
    this.sampleMatchData.matchInfo.serverName = 'Official London 8v8';
    this.sampleMatchData.matchInfo.mapName = '8v8_vienna';
    this.sampleMatchData.matchInfo.serverAddress = '158.546.52.32:27015';
  }

  loadJson(matchDataJson: string) {
    this.sampleMatchData = JSON.parse(matchDataJson);
    this.matchDate = new Date(this.sampleMatchData.matchInfo.startTime * 1000);
  }
}
