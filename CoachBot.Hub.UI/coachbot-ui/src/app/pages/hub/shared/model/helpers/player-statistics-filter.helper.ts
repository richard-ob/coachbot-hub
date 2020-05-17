import { PagedPlayerStatisticsRequestDto, PlayerStatisticFilters } from '../dtos/paged-player-statistics-request-dto.model';

// tslint:disable-next-line:no-namespace
export namespace PlayerStatisticsFilterHelper {
    export function generatePlayerStatisticsFilter(page: number, sortBy: string, sortOrder: string, filters: PlayerStatisticFilters) {
        const pagedPlayerStatisticsRequestDto = new PagedPlayerStatisticsRequestDto();
        pagedPlayerStatisticsRequestDto.filters = filters;
        pagedPlayerStatisticsRequestDto.page = page;
        pagedPlayerStatisticsRequestDto.sortOrder = sortOrder;
        if (sortBy !== null) {
            pagedPlayerStatisticsRequestDto.sortBy = sortBy;
        }

        return pagedPlayerStatisticsRequestDto;
    }
}
