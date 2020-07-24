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
import { ServerManualComponent } from '@pages/server-manual/server-manual.component';
import { NewsComponent } from '@pages/news/news.component';
import { SpinnerModule } from '@core/components/spinner/spinner.module';
import { NgxSkeletonLoaderModule } from 'ngx-skeleton-loader';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MediaComponent } from '@pages/media/media.component';
import { LightboxModule } from 'ngx-lightbox';
import { TroubleshootingComponent } from '@pages/troubleshooting/troubleshooting.component';

@NgModule({
  declarations: [
    AppComponent,
    HeroComponent,
    ManualComponent,
    TeamComponent,
    MatchmakingComponent,
    AboutComponent,
    ServerManualComponent,
    NewsComponent,
    MediaComponent,
    TroubleshootingComponent
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
    LayoutModule,
    SpinnerModule,
    NgxSkeletonLoaderModule,
    MatSnackBarModule,
    LightboxModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
