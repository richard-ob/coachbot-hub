import { Tournament } from './tournament.model';

export class TournamentSeries {
    id: number;
    name: string;
    isPublic: boolean;
    isActive: boolean;
    organisationId: number;
    tournaments: Tournament[];
}
