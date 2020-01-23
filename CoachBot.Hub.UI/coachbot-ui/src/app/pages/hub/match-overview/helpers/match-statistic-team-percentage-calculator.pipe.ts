import { Pipe, PipeTransform } from '@angular/core';
import { TeamElement } from '../model/match-data.interface';
import { TeamType } from '../model/team-type.enum';
import { StatisticType } from '../model/statistic-type.enum';

@Pipe({ name: 'matchStatisticTeamPercentage' })
export class MatchStatisticTeamPercentageCalculatorPipe implements PipeTransform {
    transform(
        teams: TeamElement[], statisticType: StatisticType, comparisonStatisticType: StatisticType, teamType: TeamType = TeamType.Home
    ): string {
        const teamStat = teams[teamType].matchTotal.statistics[statisticType];
        const totalStat = teamStat + teams[teamType].matchTotal.statistics[comparisonStatisticType];

        return ((teamStat / totalStat) * 100).toFixed(0);
    }
}
