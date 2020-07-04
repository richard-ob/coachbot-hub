import { TeamStatisticFilters, PagedTeamStatisticsRequestDto } from '../dtos/paged-team-statistics-request-dto.model';

// tslint:disable-next-line:no-namespace
export namespace TeamStatisticsFilterHelper {
    export function generatePlayerStatisticsFilter(
        page: number, pageSize: number, sortBy: string, sortOrder: string, filters: TeamStatisticFilters
    ) {
        const pagedTeamStatisticsRequestDto = new PagedTeamStatisticsRequestDto();
        pagedTeamStatisticsRequestDto.filters = filters;
        pagedTeamStatisticsRequestDto.page = page;
        pagedTeamStatisticsRequestDto.pageSize = pageSize;
        pagedTeamStatisticsRequestDto.sortOrder = sortOrder;
        if (sortBy !== null) {
            pagedTeamStatisticsRequestDto.sortBy = sortBy;
        }

        return pagedTeamStatisticsRequestDto;
    }
}
