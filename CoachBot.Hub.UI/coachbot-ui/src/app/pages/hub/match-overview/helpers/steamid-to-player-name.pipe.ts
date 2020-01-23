import { Pipe, PipeTransform } from '@angular/core';
import { Player } from '../model/match-data.interface';

@Pipe({ name: 'steamIdToPlayerName' })
export class SteamIdToPlayerNamePipe implements PipeTransform {
    transform(steamId: string, players: Player[]): string {
        return players.find(p => p.info.steamId === steamId).info.name;
    }
}
