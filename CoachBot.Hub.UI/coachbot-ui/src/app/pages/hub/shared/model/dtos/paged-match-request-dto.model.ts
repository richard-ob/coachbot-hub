export class PagedMatchRequestDto {
    page = 1;
    pageSize = 10;
    regionId: number;
    playerId?: number;
    teamId?: number;
    tournamentEditionId?: number;
    includeUpcoming = false;
    includePast = false;
    dateFrom?: Date;
    dateTo?: Date;
}
