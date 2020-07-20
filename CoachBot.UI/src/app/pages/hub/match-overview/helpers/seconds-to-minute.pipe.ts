import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'secondsToMinutes' })
export class SecondsToMinutesPipe implements PipeTransform {
    transform(seconds: number): number {
        return Math.floor(seconds / 60) + 1; // INFO: Gives the match minute, e.g. 30 seconds after kick off is the first minute
    }
}
