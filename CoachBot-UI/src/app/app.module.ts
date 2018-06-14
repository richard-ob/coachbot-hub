import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';

import { AppComponent } from './app.component';
import { ConfigurationService } from './shared/services/configuration.service';
import { ChatService } from './shared/services/chat.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    FormsModule
  ],
  providers: [ConfigurationService, ChatService],
  bootstrap: [AppComponent]
})
export class AppModule { }
