import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { PlayerListComponent } from './player-list.component';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
import { ThSorterModule } from '@shared/components/th-sorter/th-sort.module';
import { PlayerSpotlightComponent } from './player-spotlight/player-spotlight.component';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@NgModule({
    declarations: [
        PlayerListComponent,
        PlayerSpotlightComponent
    ],
    exports: [
        PlayerListComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        NgxSkeletonLoaderModule,
        ThSorterModule,
        BsDropdownModule.forRoot()
    ]
})
export class PlayerListModule { }
