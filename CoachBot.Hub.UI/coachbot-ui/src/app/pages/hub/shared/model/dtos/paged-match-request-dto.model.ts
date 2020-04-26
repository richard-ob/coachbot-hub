export class PagedMatchRequestDto {
    page = 1;
    pageSize = 10;
    regionId: number;
    playerId?: number;
    teamId?: number;
    upcomingOnly = false;
}
