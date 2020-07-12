import { Component, OnInit } from '@angular/core';
import { Player } from '../shared/model/player.model';
import { PlayerService } from '../shared/services/player.service';
import { SteamUserProfile } from '../shared/model/steam-user-profile.model';
import { CountryService } from '../shared/services/country.service';
import { Country } from '../shared/model/country.model';
import { PositionService } from '../shared/services/position.service';
import { Position } from '../shared/model/position';
import { DiscordService } from '../shared/services/discord.service';
import { DiscordUser } from '../shared/model/discord-user.model';

@Component({
    selector: 'app-profile-editor',
    templateUrl: './profile-editor.component.html',
    styleUrls: ['./profile-editor.component.scss']
})
export class ProfileEditorComponent implements OnInit {

    player: Player;
    steamUserProfile: SteamUserProfile;
    discordUser: DiscordUser;
    countries: Country[];
    positions: Position[];
    isLoading = true;

    constructor(
        private playerService: PlayerService,
        private countryService: CountryService,
        private discordService: DiscordService,
        private positionService: PositionService
    ) { }

    ngOnInit() {
        this.countryService.getCountries().subscribe(countries => {
            this.countries = countries;
            this.playerService.getCurrentPlayer().subscribe(player => {
                this.player = player;
                this.positionService.getPositions().subscribe(positions => {
                    this.positions = positions;
                    if (this.player.discordUserId && this.player.discordUserId.length) {
                        this.discordService.getUser(this.player.discordUserId).subscribe(discordUser => {
                            this.discordUser = discordUser;
                            this.isLoading = false;
                        });
                    } else {
                        this.isLoading = false;
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

    getCountryCode(countryId: number) {
        const country = this.countries.find(c => c.id === countryId);

        if (!country) {
            return;
        }

        return country.code.toLowerCase();
    }
}
