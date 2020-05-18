import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeroComponent } from './pages/hero/hero.component';
import { HubModule } from './pages/hub/hub.module';
import { LoginModule } from './pages/login/login.module';
import { ErrorModule } from './pages/error/error.module';
import { HttpClientModule } from '@angular/common/http';
import { CoreModule } from './core/core.module';
import { ManualComponent } from './pages/manual/manual.component';
import { TeamComponent } from './pages/team/team.component';
import { MatchmakingComponent } from './pages/matchmaking/matchmaking.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { LayoutModule } from '@angular/cdk/layout';
import { AboutComponent } from '@pages/about/about.component';

@NgModule({
  declarations: [
    AppComponent,
    HeroComponent,
    ManualComponent,
    TeamComponent,
    MatchmakingComponent,
    AboutComponent
  ],
  imports: [
    BrowserModule,
    CoreModule,
    AppRoutingModule,
    HttpClientModule,
    HubModule,
    LoginModule,
    ErrorModule,
    BrowserAnimationsModule,
    SweetAlert2Module.forRoot(),
    LayoutModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
