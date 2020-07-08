import { Tournament } from './tournament.model';
import { Organisation } from './organisation.model';

export class TournamentSeries {
    id: number;
    name: string;
    isPublic: boolean;
    isActive: boolean;
    organisationId: number;
    tournaments: Tournament[];
    organisation: Organisation;
}
