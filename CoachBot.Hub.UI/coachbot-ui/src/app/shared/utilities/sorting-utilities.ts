
export default class SortingUtils {
    static getSortOrder(currentSortColumn: string, newSortColumn: string, currentSortOrder: string) {
        if (newSortColumn !== null && currentSortColumn !== null && currentSortColumn === newSortColumn && currentSortOrder === 'ASC') {
            return 'DESC';
        } else {
            return 'ASC';
        }
    }
}
