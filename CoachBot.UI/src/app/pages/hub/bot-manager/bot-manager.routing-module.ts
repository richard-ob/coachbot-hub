import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BotManagerComponent } from './bot-manager.component';

const routes: Routes = [
    {
        path: 'bot',
        component: BotManagerComponent,
        data: { title: 'Bot Manager' }
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class BotManagerRoutingModule { }
