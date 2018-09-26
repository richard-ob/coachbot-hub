import { Routes } from '@angular/router';
import { ChannelsComponent } from './channels/channels.component';
import { ChannelComponent } from './channel/channel.component';
import { ServersComponent } from './servers/servers.component';
import { AnnouncementsComponent } from './announcements/announcements.component';
import { BotComponent } from './bot/bot.component';
import { LoginComponent } from './login/login.component';
import { DiscordCommandsComponent } from './discord-commands/discord-commands.component';
import { MatchHistoryComponent } from './match-history/match-history.component';
import { PlayerLeaderboardsComponent } from './player-leaderboards/player-leaderboards.component';
import { ProfileComponent } from './profile/profile.component';
import { ErrorComponent } from './shared/components/error.component';
import { RegionsComponent } from './regions/regions.component';

export const appRoutes: Routes = [
    {
        path: '',
        redirectTo: '/profile',
        pathMatch: 'full'
    },
    { path: 'channels', component: ChannelsComponent },
    { path: 'channel/:id', component: ChannelComponent },
    { path: 'servers', component: ServersComponent },
    { path: 'regions', component: RegionsComponent },
    { path: 'match-history', component: MatchHistoryComponent },
    { path: 'player-leaderboards', component: PlayerLeaderboardsComponent },
    { path: 'bot', component: BotComponent },
    { path: 'profile', component: ProfileComponent },
    { path: 'announcements', component: AnnouncementsComponent },
    { path: 'login', component: LoginComponent },
    { path: 'error', component: ErrorComponent },
    { path: 'discord-commands', component: DiscordCommandsComponent }
];
