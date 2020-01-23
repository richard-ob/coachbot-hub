import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatchOverviewComponent } from './match-overview/match-overview.component';
import { CountryNameConverterPipe } from './match-overview/helpers/country-name-converter.pipe';
import { SteamIdToPlayerNamePipe } from './match-overview/helpers/steamid-to-player-name.pipe';
import { SecondsToMinutesPipe } from './match-overview/helpers/seconds-to-minute.pipe';
import { ArrayFilterPipe } from './match-overview/helpers/array-filter.pipe';
import { MatchStatisticPercentageCalculatorPipe } from './match-overview/helpers/match-statistic-percentage-calculator.pipe';
import { MatchStatisticTeamPercentageCalculatorPipe } from './match-overview/helpers/match-statistic-team-percentage-calculator.pipe';
import { HorizontalBarGraphComponent } from './match-overview/components/horizontal-bar-graph/horizontal-bar-graph.component';
import { PercentageSharePipe } from './match-overview/helpers/percentage-share.pipe';

@NgModule({
  declarations: [
    MatchOverviewComponent,
    CountryNameConverterPipe,
    SteamIdToPlayerNamePipe,
    SecondsToMinutesPipe,
    ArrayFilterPipe,
    MatchStatisticPercentageCalculatorPipe,
    MatchStatisticTeamPercentageCalculatorPipe,
    HorizontalBarGraphComponent,
    PercentageSharePipe
  ],
  imports: [
    CommonModule
  ]
})
export class HubModule { }
