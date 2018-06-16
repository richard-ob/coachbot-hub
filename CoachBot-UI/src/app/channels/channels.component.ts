import { Component } from '@angular/core';
import { ConfigurationService } from '../shared/services/configuration.service';
import { ChatService } from '../shared/services/chat.service';
import { MatchmakerService } from '../shared/services/matchmaker.service';
import { Channel } from '../model/channel';

@Component({
    selector: 'app-channels',
    templateUrl: './channels.component.html'
})
export class ChannelsComponent {

    channels: Channel[];

    constructor(private configurationService: ConfigurationService, private chatService: ChatService, private matchmakerService: MatchmakerService) {
        this.matchmakerService.getChannels().subscribe(channels => this.channels = channels);
    }

}
