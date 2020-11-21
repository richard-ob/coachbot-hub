import { EventEmitter, Output, Input, Component } from '@angular/core';

@Component({
    // tslint:disable-next-line:component-selector
    selector: '[sorter]',
    template: `
        <span style="cursor: pointer; white-space: nowrap; text-align: center;" (click)="sort.emit(sortByColumn)">
            {{sortByColumnName || sortByColumn}}&nbsp;
            <i *ngIf="currentSort === sortByColumn && currentOrder === 'ASC'"
                class="fas fa-sort-up"></i>
            <i *ngIf="currentSort === sortByColumn && currentOrder === 'DESC'"
                class="fas fa-sort-down"></i>
            <i *ngIf="currentSort !== sortByColumn" class="fas fa-sort"></i>
            <sup *ngIf="average" style="float: left; text-align: center; width: 100%; top: 2px; left: -3px; color: #6c757d73;" i18n="@@globals.average">
                Average
            </sup>
        </span>
    `
})
export class ThSorterComponent {
    @Output() sort = new EventEmitter<string>();
    @Input() sortByColumn: string;
    @Input() sortByColumnName: string;
    @Input() currentSort: string;
    @Input() currentOrder: string;
    @Input() average = false;
}
