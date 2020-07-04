import { Pipe, PipeTransform } from '@angular/core';
import { TeamElement } from '../model/match-data.interface';
import { TeamType } from '../model/team-type.enum';
import { StatisticType } from '../model/statistic-type.enum';

@Pipe({ name: 'matchStatisticPercentage' })
export class MatchStatisticPercentageCalculatorPipe implements PipeTransform {
    transform(teams: TeamElement[], statistcType: StatisticType, teamType: TeamType = TeamType.Home): string {
        const teamStat = teams[teamType].matchTotal.statistics[statistcType];
        const totalStat =
            teams[TeamType.Home].matchTotal.statistics[statistcType]
            + teams[TeamType.Away].matchTotal.statistics[statistcType];
        return ((teamStat / totalStat) * 100).toFixed(2);
    }
}
