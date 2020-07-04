import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BotManagerComponent } from './bot-manager.component';
import { BotLogsComponent } from './bot-logs/bot-logs.component';
import { BotConnectionStateComponent } from './bot-connection-state/bot-connection-state.component';

const routes: Routes = [
    {
        path: 'bot',
        component: BotManagerComponent,
        data: { title: 'Bot Manager' },
        children: [
            { path: '', redirectTo: 'connection-state', pathMatch: 'full' },
            { path: 'connection-state', component: BotConnectionStateComponent },
            { path: 'logs', component: BotLogsComponent, }
        ]
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class BotManagerRoutingModule { }
