import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { StatisticsLeaderboardComponent } from './statistics-leaderboard.component';

@NgModule({
    declarations: [
        StatisticsLeaderboardComponent
    ],
    imports: [
        CommonModule,
        SpinnerModule,
        RouterModule,
        FormsModule
    ],
    exports: [
        StatisticsLeaderboardComponent
    ]
})
export class StatisticsLeaderboardModule { }
