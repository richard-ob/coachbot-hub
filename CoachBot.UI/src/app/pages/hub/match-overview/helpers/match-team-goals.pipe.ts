import { Pipe, PipeTransform } from '@angular/core';
import { MatchEvent, TeamEnum, EventType } from '../model/match-data.interface';

@Pipe({ name: 'matchTeamGoals' })
export class MatchTeamGoalsPipe implements PipeTransform {
    transform(matchEvents: MatchEvent[], team: TeamEnum): MatchEvent[] {

        const goalsScored = matchEvents.filter(m => m.team === team && m.event === EventType.Goal);
        const ownGoalsFor = matchEvents.filter(m => m.team !== team && m.event === EventType.OwnGoal);
        const events = goalsScored.concat(ownGoalsFor);

        return events.sort((a, b) => a.second - b.second);
    }
}
