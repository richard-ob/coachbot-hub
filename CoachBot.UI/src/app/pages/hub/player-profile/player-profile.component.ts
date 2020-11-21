import { Component, OnInit, ViewChild } from '@angular/core';
import { Player } from '../shared/model/player.model';
import { PlayerService } from '../shared/services/player.service';
import { ActivatedRoute } from '@angular/router';
import { SteamService } from '../../../shared/services/steam.service.';
import { SteamUserProfile } from '../shared/model/steam-user-profile.model';
import { PlayerProfile } from '../shared/model/player-profile.model';
import ColourUtils from '@shared/utilities/colour-utilities';
import { SwalPortalTargets, SwalComponent } from '@sweetalert2/ngx-sweetalert2';
import * as humanizeDuration from 'humanize-duration';
import { PlayerHubRole } from '../shared/model/player-hub-role.enum';

@Component({
    selector: 'app-player-profile',
    templateUrl: './player-profile.component.html',
    styleUrls: ['./player-profile.component.scss']
})
export class PlayerProfileComponent implements OnInit {

    @ViewChild('editRatingModal') editServerModal: SwalComponent;
    currentPlayer: Player;
    player: Player;
    playerProfile: PlayerProfile;
    steamUserProfile: SteamUserProfile;
    playerHubRoles = PlayerHubRole;
    playingTime: string;
    isLoading = true;

    constructor(
        private playerService: PlayerService,
        private steamService: SteamService,
        private route: ActivatedRoute,
        public readonly swalTargets: SwalPortalTargets
    ) { }

    ngOnInit() {
        this.playerService.getCurrentPlayer().subscribe(currentPlayer => this.currentPlayer = currentPlayer);
        this.route.paramMap.pipe().subscribe(params => {
            this.playerService.getPlayer(+params.get('id')).subscribe(player => {
                this.player = player;
                this.playerService.getPlayerProfile(player.id).subscribe(playerProfile => {
                    this.playerProfile = playerProfile;
                    if (this.player.steamID) {
                        this.steamService.getUserProfiles([this.player.steamID]).subscribe(steamUserProfile => {
                            this.steamUserProfile = steamUserProfile.response.players[0];
                            this.steamService.getPlayingTime(this.player.steamID).subscribe(steamPlayingTime => {
                                if (steamPlayingTime && steamPlayingTime.response && steamPlayingTime.response.games) {
                                    const iosoccerStats = steamPlayingTime.response.games.find(g => g.appid === 673560);
                                    if (iosoccerStats && iosoccerStats.playtime_forever) {
                                        this.playingTime = humanizeDuration(iosoccerStats.playtime_forever * 60 * 1000);
                                    }
                                }
                                this.isLoading = false;
                            });
                        });
                    } else {
                        this.steamUserProfile = null;
                        this.isLoading = false;
                    }
                });
            });
        });
    }

    getBackground() {
        if (!this.playerProfile || !this.playerProfile.clubTeam || !this.playerProfile.clubTeam.color) {
            return this.generateGradient('#292c31');
        }
        return this.generateGradient(this.playerProfile.clubTeam.color);
    }

    generateGradient(colour: string) {
        const gradientSrc =
            'linear-gradient(90deg,' + ColourUtils.hexToRgbA(colour, 0.6) + ',' + ColourUtils.hexToRgbA(colour, 0.3) + ')';
        return gradientSrc;
    }

    updateRating() {
        this.isLoading = true;
        this.playerService.updatePlayerRating(this.player).subscribe(() => {
            this.isLoading = false;
        });
    }
}
