import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { RecentMatchesComponent } from './recent-matches.component';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { NgxPaginationModule } from 'ngx-pagination';
import { TeamNameDisplayModule } from '../shared/components/team-name-display/team-name-display.module';
import { FormIndicatorModule } from '../shared/components/form-indictator/form-indicator.module';

@NgModule({
    declarations: [
        RecentMatchesComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        MatDatepickerModule,
        MatNativeDateModule,
        NgxPaginationModule,
        TeamNameDisplayModule,
        FormIndicatorModule
    ],
    exports: [
        RecentMatchesComponent
    ]
})
export class RecentMatchesModule { }
