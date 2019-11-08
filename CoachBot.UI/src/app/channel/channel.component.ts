import { Component } from '@angular/core';
import { ChannelService, } from '../shared/services/channel.service';
import { Channel } from '../model/channel';
import { ChannelPosition } from '../model/channel-position';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';
import { Region } from '../model/region';
import { RegionService } from '../shared/services/region.service';
import { Position } from '../model/position';

@Component({
    selector: 'app-channel',
    templateUrl: './channel.component.html'
})
export class ChannelComponent {

    channel: Channel;
    regions: Region[];
    svgKitColor: string;
    editPositionName: string;
    currentPositionId: number;
    isSaving = false;

    constructor(private route: ActivatedRoute,
        private channelService: ChannelService,
        private regionService: RegionService) {
        this.regionService.getRegions().subscribe(regions => this.regions = regions);
        this.route.params
            .pipe(map(params => params['id']))
            .subscribe((id) => {
                this.channelService
                    .getChannel(id)
                    .subscribe(channel => {
                        this.channel = channel;
                        this.channel.channelPositions = this.channel.channelPositions || [];
                    });
            });
    }

    saveChannel() {
        this.isSaving = true;
        this.channelService.updateChannel(this.channel).subscribe(() => this.isSaving = false);
    }

    addPosition() {
        const position = new ChannelPosition();
        position.position = new Position();
        this.channel.channelPositions.push(position);
    }

    removePosition() {
        this.channel.channelPositions.pop();
    }

    editPosition(positionId: number) {
        this.editPositionName = this.channel.channelPositions[positionId].position.name;
        this.currentPositionId = positionId;
    }

    savePositionName() {
        this.channel.channelPositions[this.currentPositionId].position.name = this.editPositionName;
    }

    getKitUrl(): string {
        if (this.channel.emotes) {
            const emote = this.channel.emotes.find(e => e.key === this.channel.team1.kitEmote);
            if (emote) {
                return emote.value;
            } else {
                return null;
            }
        }
    }

    getBadgeUrl(): string {
        if (this.channel.emotes) {
            const emote = this.channel.emotes.find(e => e.key === this.channel.team1.badgeEmote);
            if (emote) {
                return emote.value;
            } else {
                return null;
            }
        }
    }
}
