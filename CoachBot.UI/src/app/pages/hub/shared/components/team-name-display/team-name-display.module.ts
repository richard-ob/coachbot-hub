import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { TeamNameDisplayComponent } from './team-name-display.component';

@NgModule({
    declarations: [
        TeamNameDisplayComponent
    ],
    imports: [
        CommonModule,
        SpinnerModule,
        RouterModule,
        FormsModule
    ],
    exports: [
        TeamNameDisplayComponent
    ]
})
export class TeamNameDisplayModule { }
