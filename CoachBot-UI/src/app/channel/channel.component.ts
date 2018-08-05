import { Component } from '@angular/core';
import { MatchmakerService } from '../shared/services/matchmaker.service';
import { Channel } from '../model/channel';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { map } from 'rxjs/operators';

@Component({
    selector: 'app-channel',
    templateUrl: './channel.component.html'
})
export class ChannelComponent {

    channel: Channel;

    constructor(private route: ActivatedRoute,
        private matchmakerService: MatchmakerService) {
        this.route.params
            .pipe(map(params => params['id']))
            .subscribe((id) => {
                this.matchmakerService
                    .getChannels()
                    .subscribe(channels => {
                        this.channel = channels[0];
                    });
            });
    }

    saveChannel() {
        this.matchmakerService.updateChannel(this.channel);
    }
}
