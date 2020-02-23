import { Component, OnInit, HostListener } from '@angular/core';
import { Player } from '../shared/model/player.model';
import { PlayerService } from '../shared/services/player.service';
import { ActivatedRoute } from '@angular/router';
import { SteamUserProfile } from '../shared/model/steam-user-profile.model';

@Component({
    selector: 'app-profile-editor',
    templateUrl: './profile-editor.component.html',
    styleUrls: ['./profile-editor.component.scss']
})
export class ProfileEditorComponent implements OnInit {

    player: Player;
    steamUserProfile: SteamUserProfile;
    playingTime: string;
    isLoading = true;

    constructor(private playerService: PlayerService, private route: ActivatedRoute) { }

    ngOnInit() {
    }

}
