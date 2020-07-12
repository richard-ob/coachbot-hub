import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { PlayerSelectorComponent } from './player-selector.component';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@NgModule({
    declarations: [
        PlayerSelectorComponent
    ],
    imports: [
        CommonModule,
        SpinnerModule,
        RouterModule,
        FormsModule
    ],
    exports: [
        PlayerSelectorComponent
    ]
})
export class PlayerSelectorModule { }
