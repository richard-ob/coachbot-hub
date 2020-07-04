import { Tournament } from './tournament.model';

export class TournamentMatchDaySlot {
    id?: number;
    tournamentId: number;
    tournament?: Tournament;
    matchDay: TournamentMatchDay;
    matchTime: Date;
    createdDate?: Date;
}

export enum TournamentMatchDay {
    Monday = 0,
    Tuesday,
    Wednesday,
    Thursday,
    Friday,
    Saturday,
    Sunday
}
