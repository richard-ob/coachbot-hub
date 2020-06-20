import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { FantasyTeamEditorComponent } from './fantasy-team-editor/fantasy-team-editor.component';
import { FantasyTeamEditorPlayersComponent } from './fantasy-team-editor/fantasy-team-editor-players/fantasy-team-editor-players.component';
import { NgPipesModule } from 'ngx-pipes';
import { NouisliderModule } from 'ng2-nouislider';
import { HubPipesModule } from '@pages/hub/shared/pipes/hub-pipes.module';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { FantasyPlayerComponent } from './fantasy-team-editor/fantasy-player/fantasy-player.component';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { FantasyTeamManagerRoutingModule } from './fantasy.routing-module';
import { FantasyTeamManagerComponent } from './fantasy-team-manager/fantasy-team-manager.component';
import { FantasyOverviewComponent } from './fantasy-overview/fantasy-overview.component';
import { FantasyTeamOverviewComponent } from './fantasy-team-overview/fantasy-team-overview.component';

@NgModule({
    declarations: [
        FantasyTeamManagerComponent,
        FantasyTeamEditorComponent,
        FantasyTeamEditorPlayersComponent,
        FantasyPlayerComponent,
        FantasyOverviewComponent,
        FantasyTeamOverviewComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        FantasyTeamManagerRoutingModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        NgPipesModule,
        NouisliderModule,
        HubPipesModule,
        MatSnackBarModule,
        SweetAlert2Module
    ]
})
export class FantasyTeamManagerModule { }
