import { Component, Output, EventEmitter } from '@angular/core';
import { Player } from '../../model/player.model';
import { PlayerService } from '../../services/player.service';

@Component({
    selector: 'app-player-selector',
    templateUrl: './player-selector.component.html'
})
export class PlayerSelectorComponent {

    @Output() playerSelected = new EventEmitter<Player>();
    playerSearchResults: Player[];
    selectedPlayer: Player;
    searchQuery = '';
    isSearching = false;

    constructor(private playerService: PlayerService) {

    }

    selectPlayer(player: Player) {
        this.playerSelected.emit(player);
        this.selectedPlayer = player;
        this.playerSearchResults = null;
        this.searchQuery = null;
    }

    deselectPlayer() {
        this.selectedPlayer = null;
        this.playerSelected.emit(null);
    }

    search() {
        this.selectedPlayer = null;
        this.isSearching = true;
        this.playerSearchResults = null;
        this.playerService.searchPlayerByName(this.searchQuery).subscribe(
            players => {
                this.playerSearchResults = players;
                this.isSearching = false;
            }
        );
    }

}
