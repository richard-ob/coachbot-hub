import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { CoreModule } from '@core/core.module';
import { BotManagerComponent } from './bot-manager.component';
import { BotManagerRoutingModule } from './bot-manager.routing-module';

@NgModule({
    declarations: [
        BotManagerComponent
    ],
    imports: [
        CommonModule,
        CoreModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        BotManagerRoutingModule
    ]
})
export class BotManagerModule { }
