import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { RouterModule, Routes } from '@angular/router';

import { AppComponent } from './app.component';
import { ConfigurationService } from './shared/services/configuration.service';
import { ChatService } from './shared/services/chat.service';
import { MatchmakerService } from './shared/services/matchmaker.service';
import { ChannelComponent } from './channel/channel.component';

import { appRoutes } from './app.routes';
import { ChannelsComponent } from './channels/channels.component';

@NgModule({
  declarations: [
    AppComponent,
    ChannelComponent,
    ChannelsComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot(appRoutes)
  ],
  providers: [
    ConfigurationService,
    ChatService,
    MatchmakerService
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
