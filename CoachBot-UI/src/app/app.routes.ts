import { Routes } from "@angular/router";
import { ChannelsComponent } from "./channels/channels.component";
import { ChannelComponent } from "./channel/channel.component";

export const appRoutes: Routes = [
    {
        path: '',
        redirectTo: '/channels',
        pathMatch: 'full'
    },
    { path: 'channels', component: ChannelsComponent },
    { path: 'channel/:id', component: ChannelComponent }
];