import { Match } from './match.model';
import { Player } from './player.model';
import { TournamentPhase } from './tournament-phase.model';

export class ScorePrediction {
    id: number;
    homeGoalsPrediction = 0;
    awayGoalsPrediction = 0;
    matchId: number;
    match?: Match;
    playerId: number;
    player?: Player;
    tournamentPhaseId: number;
    tournamentPhase: TournamentPhase;
    updatedDate: Date;
    createdDate: Date;
}
