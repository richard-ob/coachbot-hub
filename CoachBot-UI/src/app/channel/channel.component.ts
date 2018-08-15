import { Component } from '@angular/core';
import { MatchmakerService } from '../shared/services/matchmaker.service';
import { Channel } from '../model/channel';
import { Position } from '../model/position';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { map } from 'rxjs/operators';
import { DomSanitizer } from '@angular/platform-browser';

@Component({
    selector: 'app-channel',
    templateUrl: './channel.component.html'
})
export class ChannelComponent {

    channel: Channel;
    svgKitColor: string;
    editPositionName: string;
    currentPositionId: number;
    isSaving = false;

    constructor(private route: ActivatedRoute,
        private matchmakerService: MatchmakerService, private _sanitizer: DomSanitizer) {
        this.route.params
            .pipe(map(params => params['id']))
            .subscribe((id) => {
                this.matchmakerService
                    .getChannel(id)
                    .subscribe(channel => {
                        this.channel = channel;
                        this.channel.id = this.channel.idString;
                    });
            });
    }

    saveChannel() {
        this.isSaving = true;
        this.matchmakerService.updateChannel(this.channel).subscribe(complete => this.isSaving = false);
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
}
