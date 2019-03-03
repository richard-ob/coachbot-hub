import { Component } from '@angular/core';
import { ChannelService, } from '../shared/services/channel.service';
import { Channel } from '../model/channel';
import { Position } from '../model/position';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs/operators';
import { Region } from '../model/region';
import { RegionService } from '../shared/services/region.service';

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
                        this.channel.id = this.channel.idString;
                    });
            });
    }

    saveChannel() {
        this.isSaving = true;
        this.channelService.updateChannel(this.channel).subscribe(complete => this.isSaving = false);
    }

    addPosition() {
        const position = new Position();
        position.positionName = (this.channel.positions.length + 1).toString();
        this.channel.positions.push(position);
    }

    removePosition() {
        this.channel.positions.pop();
    }

    editPosition(positionId: number) {
        this.editPositionName = this.channel.positions[positionId].positionName;
        this.currentPositionId = positionId;
    }

    savePositionName() {
        this.channel.positions[this.currentPositionId].positionName = this.editPositionName;
    }

    getKitUrl(): string {
        const emote = this.channel.emotes.find(e => e.key === this.channel.team1.kitEmote);
        if (emote) {
            return emote.value;
        } else {
            return null;
        }
    }

    getBadgeUrl(): string {
        const emote = this.channel.emotes.find(e => e.key === this.channel.team1.badgeEmote);
        if (emote) {
            return emote.value;
        } else {
            return null;
        }
    }
}
