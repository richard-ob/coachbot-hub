import { MatchOutcomeType } from './match-outcome-type.enum';

export interface TournamentGroupStanding {
    goalsConceded: number;
    goalsScored: number;
    goalDifference: number;
    wins: number;
    losses: number;
    draws: number;
    matches: number;
    points: number;
    position: number;
    badgeImageUrl: string;
    teamName: string;
    teamId: number;
    teamCode: string;
    form: MatchOutcomeType[];
}
