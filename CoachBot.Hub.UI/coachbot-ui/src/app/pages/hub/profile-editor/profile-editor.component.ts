import { Component, OnInit, HostListener } from '@angular/core';
import { Player } from '../shared/model/player.model';
import { PlayerService } from '../shared/services/player.service';
import { SteamUserProfile } from '../shared/model/steam-user-profile.model';
import { CountryService } from '../shared/services/country.service';
import { Country } from '../shared/model/country.model';
import { SteamService } from '../../../shared/services/steam.service.';
import { PositionService } from '../shared/services/position.service';
import { Position } from '../shared/model/position';
import { PlayerPosition } from '../shared/model/player-position.model';
import { PlayerTeamService } from '../shared/services/player-team.service';
import { PlayerTeam } from '../shared/model/player-team.model';

@Component({
    selector: 'app-profile-editor',
    templateUrl: './profile-editor.component.html',
    styleUrls: ['./profile-editor.component.scss']
})
export class ProfileEditorComponent implements OnInit {

    player: Player;
    steamUserProfile: SteamUserProfile;
    countries: Country[];
    positions: Position[];
    isLoading = true;

    constructor(
        private playerService: PlayerService,
        private countryService: CountryService,
        private steamService: SteamService,
        private positionService: PositionService
    ) { }

    ngOnInit() {
        this.countryService.getCountries().subscribe(countries => {
            this.countries = countries;
            this.playerService.getCurrentPlayer().subscribe(player => {
                this.player = player;
                this.positionService.getPositions().subscribe(positions => {
                    this.positions = positions;
                    this.isLoading = false;
                    if (this.player.steamID && this.player.steamID.length > 5) {
                        this.steamService.getUserProfiles([this.player.steamID]).subscribe(steamResponse => {
                            if (steamResponse && steamResponse.response.players) {
                                this.steamUserProfile = steamResponse.response.players[0];
                            }
                        });
                    }
                });
            });
        });
    }

    addPosition(positionId: number) {
        const playerPosition = {
            playerId: this.player.id,
            positionId,
            position: this.positions.find(p => p.id === positionId)
        };
        if (!this.player.positions) {
            this.player.positions = [];
        }
        this.player.positions.push(playerPosition);
    }

    saveChanges() {
        this.isLoading = true;
        this.playerService.updateCurrentPlayer(this.player).subscribe(() => {
            this.playerService.getCurrentPlayer().subscribe(player => {
                this.player = player;
                this.isLoading = false;
            });
        });
    }

    startDiscordVerification() {
        window.location.href = 'http://localhost/verify-discord';
    }
}
