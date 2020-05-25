import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { FormIndicatorModule } from '../shared/components/form-indictator/form-indicator.module';
import { ThSorterModule } from '@shared/components/th-sorter/th-sort.module';
import { TeamEditorComponent } from './team-editor.component';
import { TeamEditorDiscordIntegrationComponent } from './team-editor-discord-integration/team-editor-discord-integration.component';
import { DiscordChannelEditorComponent } from './team-editor-discord-integration/discord-channel-editor/discord-channel-editor.component';
import { DiscordGuildEditorComponent } from './team-editor-discord-integration/discord-guild-editor/discord-guild-editor.component';
import { TeamCreatorComponent } from './team-creator/team-creator.component';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { TeamEditorSquadComponent } from './team-editor-player-list/team-editor-squad.component';
import {
    TeamEditorSquadAddPlayerComponent
} from './team-editor-player-list/team-editor-squad-add-player/team-editor-squad-add-player.component';
import { DiscordEmoteDisplayNamePipe } from './team-editor-discord-integration/discord-guild-editor/discord-emote-display-name.pipe';
import { AssetImageUploaderModule } from '@shared/components/asset-image-uploader/asset-image-uploader.module';
import { ChromeColourPickerModule } from '@shared/components/chrome-colour-picker/chrome-colour-picker.module';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { HubPipesModule } from '../shared/pipes/hub-pipes.module';
import { TeamEditorRoutingModule } from './team-editor.routing-module';
import { TeamInfoEditorComponent } from './shared/team-info-editor/team-info-editor.component';
import { TeamEditorInfoComponent } from './team-editor-info/team-editor-info.component';

@NgModule({
    declarations: [
        TeamInfoEditorComponent,
        TeamEditorComponent,
        TeamCreatorComponent,
        TeamEditorInfoComponent,
        TeamEditorSquadComponent,
        TeamEditorSquadAddPlayerComponent,
        TeamEditorDiscordIntegrationComponent,
        DiscordChannelEditorComponent,
        DiscordGuildEditorComponent,
        DiscordEmoteDisplayNamePipe
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        FormIndicatorModule,
        ThSorterModule,
        SweetAlert2Module,
        AssetImageUploaderModule,
        TeamEditorRoutingModule,
        ChromeColourPickerModule,
        MatDatepickerModule,
        HubPipesModule
    ]
})
export class TeamEditorModule { }
