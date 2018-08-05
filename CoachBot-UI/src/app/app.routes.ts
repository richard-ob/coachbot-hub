import { Routes } from '@angular/router';
import { ChannelsComponent } from './channels/channels.component';
import { ChannelComponent } from './channel/channel.component';
import { ServersComponent } from './servers/servers.component';
import { AnnouncementsComponent } from './announcements/announcements.component';
import { BotComponent } from './bot/bot.component';
import { LoginComponent } from './login/login.component';

export const appRoutes: Routes = [
    {
        path: '',
        redirectTo: '/channels',
        pathMatch: 'full'
    },
    { path: 'channels', component: ChannelsComponent },
    { path: 'channel/:id', component: ChannelComponent },
    { path: 'servers', component: ServersComponent },
    { path: 'bot', component: BotComponent },
    { path: 'announcements', component: AnnouncementsComponent },
    { path: 'login', component: LoginComponent }
];
