import { Component } from '@angular/core';
import { ChannelService } from '../shared/services/channel.service';
import { Channel } from '../model/channel';
import { Team } from '../model/team';
import { ChannelPosition } from '../model/channel-position';
import { Position } from '../model/position';

@Component({
    selector: 'app-channels',
    templateUrl: './channels.component.html'
})
export class ChannelsComponent {

    channels: Channel[];
    unconfiguredChannels: Channel[];
    unconfiguredChannel: Channel;
    isLoading = true;

    constructor(private channelService: ChannelService) {
        this.loadChannels();
    }

    loadChannels() {
        this.channelService.getChannels()
            .subscribe(channels => {
                this.channels = channels;
                this.channelService.getUnconfiguredChannels()
                    .subscribe(unconfiguredChannels => {
                        this.unconfiguredChannels = unconfiguredChannels;
                        this.isLoading = false;
                    });
            });
    }

    addChannel() {
        this.isLoading = true;
        this.unconfiguredChannel.formation = 0;
        this.unconfiguredChannel.classicLineup = true;
        this.unconfiguredChannel.channelPositions = [];
        ['GK', 'LB', 'RB', 'CB', 'CM', 'LW', 'RW', 'CF'].map(positionName => {
            const channelPosition = new ChannelPosition();
            channelPosition.position = new Position();
            channelPosition.position.name = positionName;
            this.unconfiguredChannel.channelPositions.push(channelPosition)
        });
        this.unconfiguredChannel.team1 = new Team();
        this.unconfiguredChannel.name = 'Unnamed';
        this.unconfiguredChannel.color = '#2463B0';
        this.unconfiguredChannel.team1.isMix = false;
        this.unconfiguredChannel.team1.players = [];
        this.unconfiguredChannel.team2 = new Team();
        this.unconfiguredChannel.team2.players = [];
        this.channelService.createChannel(this.unconfiguredChannel)
            .subscribe(complete => this.loadChannels());
    }

}
