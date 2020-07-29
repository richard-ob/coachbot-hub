import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'matchDataToken', pure: true })
export class MatchDataTokenPipe implements PipeTransform {
    transform(matchDataToken: string): string {
        if (!matchDataToken) {
            return 'N/A';
        }

        return atob(matchDataToken);
    }
}
