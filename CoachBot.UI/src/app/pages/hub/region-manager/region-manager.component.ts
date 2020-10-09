import { Component } from '@angular/core';
import { Region } from '../shared/model/region.model';
import { RegionService } from '../shared/services/region.service';
import { PlayerService } from '../shared/services/player.service';
import { Player } from '../shared/model/player.model';
import { PlayerHubRole } from '../shared/model/player-hub-role.enum';

@Component({
    selector: 'app-region-manager',
    templateUrl: './region-manager.component.html'
})
export class RegionManagerComponent {

    currentPlayer: Player;
    newRegion: Region = new Region();
    regions: Region[];
    playerHubRoles = PlayerHubRole;
    isAdding = false;

    constructor(private regionService: RegionService, private playerService: PlayerService) {
        this.regionService.getRegions().subscribe(regions => this.regions = regions);
        this.playerService.getCurrentPlayer().subscribe(currentPlayer => {
            this.currentPlayer = currentPlayer;
        });
    }

    addRegion() {
        this.isAdding = true;
        this.regionService.addRegion(this.newRegion).subscribe(() => {
            this.regionService.getRegions().subscribe(regions => this.regions = regions);
            this.newRegion = new Region();
            this.isAdding = false;
        });
    }

    removeRegion(regionId: number) {
        this.regionService.removeRegion(regionId).subscribe(() => {
            this.regionService.getRegions().subscribe(regions => this.regions = regions);
        });
    }

    regenerateToken(regionId: number) {
        this.regionService.generateNewRegionToken(regionId).subscribe();
    }
}
