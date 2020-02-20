import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-th-sorter',
    template: `
        <th style="cursor: pointer" class="team-result__status"
            (click)="loadPage(currentPage, 'StatisticTotals.Losses')">
            <span style=" white-space: nowrap; ">
                    Wins&nbsp;
                <i *ngIf="sortBy === 'StatisticTotals.Losses' && sortOrder === 'ASC'"
                    class="fas fa-sort-up"></i>
                <i *ngIf="sortBy === 'StatisticTotals.Losses' && sortOrder === 'DESC'"
                    class="fas fa-sort-down"></i>
            </span>
        </th>
    `
})
export class ThSorterComponent {

}
