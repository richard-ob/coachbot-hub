import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'secondsToMinutes' })
export class SecondsToMinutesPipe implements PipeTransform {
    transform(seconds: number): number {
        return Math.floor(seconds / 60);
    }
}
