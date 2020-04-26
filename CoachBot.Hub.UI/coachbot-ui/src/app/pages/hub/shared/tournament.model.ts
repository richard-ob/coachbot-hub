import { TournamentEdition } from './model/tournament-edition.model';

export class Tournament {
    id: number;
    name: string;
    isPublic: boolean;
    isActive: boolean;
    tournamentType: number;
    organisationId: number;
    tournamentEditions: TournamentEdition[];
}
