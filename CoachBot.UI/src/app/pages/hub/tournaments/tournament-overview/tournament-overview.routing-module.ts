import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { TournamentOverviewTeamsComponent } from './tournament-overview-teams/tournament-overview-teams.component';
import { TournamentOverviewStandingsComponent } from './tournament-overview-standings/tournament-overview-standings.component';
import { TournamentOverviewFixturesComponent } from './tournament-overview-fixtures/tournament-overview-fixtures.component';
import { TournamentOverviewPlayersComponent } from './tournament-overview-players/tournament-overview-players.component';
import { TournamentOverviewStaffComponent } from './tournament-overview-staff/tournament-overview-staff.component';
import { TournamentOverviewComponent } from './tournament-overview.component';

const routes: Routes = [
    {
        path: 'tournament/:id',
        component: TournamentOverviewComponent,
        children: [
            { path: '', redirectTo: 'standings', pathMatch: 'full' },
            { path: 'standings', component: TournamentOverviewStandingsComponent },
            { path: 'fixtures', component: TournamentOverviewFixturesComponent },
            { path: 'players', component: TournamentOverviewPlayersComponent },
            { path: 'teams', component: TournamentOverviewTeamsComponent },
            { path: 'staff', component: TournamentOverviewStaffComponent }
        ],
        data: { title: $localize`:@@globals.tournamentOverview:Tournament Overview` }
    }
];
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
    providers: []
})
export class TournamentOverviewRoutingModule { }
