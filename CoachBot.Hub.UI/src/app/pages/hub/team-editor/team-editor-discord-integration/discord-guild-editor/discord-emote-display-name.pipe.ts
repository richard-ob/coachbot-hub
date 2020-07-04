import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'emoteDisplayName' })
export class DiscordEmoteDisplayNamePipe implements PipeTransform {
    transform(emoteString: string): string {
        return emoteString.split(':')[1];
    }
}
