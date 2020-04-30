import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { RecentMatchesComponent } from './recent-matches.component';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { NgxPaginationModule } from 'ngx-pagination';

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
        NgxPaginationModule
    ],
    exports: [
        RecentMatchesComponent
    ]
})
export class RecentMatchesModule { }
