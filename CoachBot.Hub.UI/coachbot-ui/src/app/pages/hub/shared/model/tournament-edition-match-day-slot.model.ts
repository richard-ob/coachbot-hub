import { TournamentEdition } from './tournament-edition.model';

export class TournamentEditionMatchDaySlot {
    id?: number;
    tournamentEditionId: number;
    tournamentEdition?: TournamentEdition;
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
