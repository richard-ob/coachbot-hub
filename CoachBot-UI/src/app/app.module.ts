import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';
import { ColorPickerModule } from 'ngx-color-picker';
import { ModalModule } from 'ngx-bootstrap/modal';
import { NgxPaginationModule } from 'ngx-pagination';

import { AppComponent } from './app.component';
import { ConfigurationService } from './shared/services/configuration.service';
import { MatchmakerService } from './shared/services/matchmaker.service';
import { ChannelComponent } from './channel/channel.component';

import { appRoutes } from './app.routes';
import { ChannelsComponent } from './channels/channels.component';
import { RequestOptionsInterceptor } from './shared/interceptors/request-options.interceptor';
import { UserService } from './shared/services/user.service';
import { ServersComponent } from './servers/servers.component';
import { AnnouncementService } from './shared/services/announcement.service';
import { AnnouncementsComponent } from './announcements/announcements.component';
import { BotComponent } from './bot/bot.component';
import { BotService } from './shared/services/bot.service';
import { UnauthorizedInterceptor } from './shared/interceptors/unauthorized.interceptor';
import { LoginComponent } from './login/login.component';
import { DiscordCommandsComponent } from './discord-commands/discord-commands.component';
import { KitComponent } from './kit/kit.component';
import { MatchService } from './shared/services/match.service';
import { MatchHistoryComponent } from './match-history/match-history.component';
import { LeaderboardService } from './shared/services/leaderboard.service';
import { PlayerLeaderboardsComponent } from './player-leaderboards/player-leaderboards.component';
import { ProfileComponent } from './profile/profile.component';
import { LogService } from './shared/services/log.service';
import { ErrorInterceptor } from './shared/interceptors/error.interceptor';
import { ErrorComponent } from './shared/components/error.component';
import { RegionsComponent } from './regions/regions.component';
import { RegionService } from './shared/services/region.service';

@NgModule({
  declarations: [
    AppComponent,
    AnnouncementsComponent,
    BotComponent,
    ChannelComponent,
    ChannelsComponent,
    DiscordCommandsComponent,
    ErrorComponent,
    PlayerLeaderboardsComponent,
    LoginComponent,
    KitComponent,
    MatchHistoryComponent,
    ServersComponent,
    ProfileComponent,
    RegionsComponent
  ],
  imports: [
    BrowserModule,
    ColorPickerModule,
    HttpClientModule,
    FormsModule,
    NgxPaginationModule,
    ModalModule.forRoot(),
    RouterModule.forRoot(appRoutes, { useHash: true })
  ],
  providers: [
    ConfigurationService,
    AnnouncementService,
    BotService,
    LeaderboardService,
    LogService,
    MatchmakerService,
    MatchService,
    RegionService,
    UserService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: RequestOptionsInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: UnauthorizedInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
