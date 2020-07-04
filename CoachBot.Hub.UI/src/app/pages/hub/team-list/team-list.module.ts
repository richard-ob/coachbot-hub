import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { TeamListComponent } from './team-list.component';
import { FormIndicatorModule } from '../shared/components/form-indictator/form-indicator.module';
import { ThSorterModule } from '@shared/components/th-sorter/th-sort.module';
import { TeamSpotlightComponent } from './team-spotlight/team-spotlight.component';

@NgModule({
    declarations: [
        TeamListComponent,
        TeamSpotlightComponent
    ],
    exports: [
        TeamListComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        FormIndicatorModule,
        ThSorterModule
    ]
})
export class TeamListModule { }
