import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

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

@NgModule({
  declarations: [
    AppComponent,
    AnnouncementsComponent,
    BotComponent,
    ChannelComponent,
    ChannelsComponent,
    ServersComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(appRoutes)
  ],
  providers: [
    ConfigurationService,
    AnnouncementService,
    BotService,
    MatchmakerService,
    UserService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: RequestOptionsInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
