import { Component } from '@angular/core';

declare var Crate;

@Component({
    selector: 'app-matchmaking',
    templateUrl: './matchmaking.component.html'
})
export class MatchmakingComponent {

    constructor() {
        const crate = new Crate({
            server: '132834484138672128',
            channel: '669659754548690974',
            shard: 'https://e.widgetbot.io'
        });
    }

}
