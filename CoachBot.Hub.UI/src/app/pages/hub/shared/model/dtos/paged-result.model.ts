export interface PagedResult<T> {
    items: T[];
    page: number;
    pageSize: number;
    sortBy: string;
    sortOrder: string;
    totalItems: number;
    totalPages: number;
}
