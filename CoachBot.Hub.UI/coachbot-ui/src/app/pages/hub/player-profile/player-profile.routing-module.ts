import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { PlayerProfileComponent } from './player-profile.component';
import { PlayerProfileStatisticsComponent } from './player-profile-statistics/player-profile-statistics.component';
import { PlayerProfileMatchesComponent } from './player-profile-matches/player-profile-matches.component';
import { PlayerTeamHistoryComponent } from './player-team-history/player-team-history.component';
import { PlayerProfileTournamentsComponent } from './player-profile-tournaments/player-profile-tournaments.component';

const routes: Routes = [
    {
        path: 'player-profile/:id',
        component: PlayerProfileComponent,
        children: [
            { path: '', redirectTo: 'statistics', pathMatch: 'full' },
            { path: 'statistics', component: PlayerProfileStatisticsComponent },
            { path: 'matches', component: PlayerProfileMatchesComponent },
            { path: 'teams', component: PlayerTeamHistoryComponent },
            { path: 'tournaments', component: PlayerProfileTournamentsComponent }
        ]
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class PlayerProfileRoutingModule { }
