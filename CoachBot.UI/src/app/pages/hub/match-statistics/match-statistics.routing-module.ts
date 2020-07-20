import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UnlinkedMatchStatisticsComponent } from './unlinked-match-statistics/unlinked-match-statistics.component';

const routes: Routes = [
    {
        path: 'unlinked-match-statistics',
        component: UnlinkedMatchStatisticsComponent,
        data: { title: 'Unlinked Match Statistics' }
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class MatchStatisticsRoutingModule { }
