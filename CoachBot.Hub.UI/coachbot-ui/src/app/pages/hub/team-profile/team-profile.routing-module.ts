import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TeamProfileComponent } from './team-profile.component';
import { TeamProfileMatchesComponent } from './team-profile-matches/team-profile-matches.component';
import { TeamProfilePlayersComponent } from './team-profile-players/team-profile-players.component';
import { TeamProfileTournamentsComponent } from './team-profile-tournaments/team-profile-tournaments.component';
import { TeamProfileStatisticsComponent } from './team-profile-statistics/team-profile-statistics.component';
import { TeamProfilePlayerHistoryComponent } from './team-profile-player-history/team-profile-player-history.component';

const routes: Routes = [
    {
        path: 'team-profile/:id',
        component: TeamProfileComponent,
        children: [
            { path: '', redirectTo: 'statistics', pathMatch: 'full' },
            { path: 'statistics', component: TeamProfileStatisticsComponent },
            { path: 'matches', component: TeamProfileMatchesComponent },
            { path: 'squad', component: TeamProfilePlayersComponent },
            { path: 'player-history', component: TeamProfilePlayerHistoryComponent },
            { path: 'tournaments', component: TeamProfileTournamentsComponent }
        ]
    },
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class TeamProfileRoutingModule { }
