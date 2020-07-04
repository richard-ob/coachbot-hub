import { Component, OnInit, Inject } from '@angular/core';
import { PlayerService } from '../shared/services/player.service';
import { ActivatedRoute } from '@angular/router';
import { DOCUMENT } from '@angular/common';

@Component({
    selector: 'app-steam-id-validator',
    templateUrl: './steam-id-validator.component.html'
})
export class SteamIDValidatorComponent implements OnInit {

    constructor(
        private playerService: PlayerService,
        private route: ActivatedRoute,
        @Inject(DOCUMENT) private document: Document
    ) { }

    ngOnInit() {
        this.route.queryParamMap.subscribe(params => {
            if (params.get('validated') === 'true' && params.get('steamId') !== '') {
                this.playerService.updateSteamId(params.get('steamId')).subscribe();
            } else {
                this.document.location.href = 'https://hub.iosoccer.co.uk/steamid-validate/?login';
            }
        });
    }

}
