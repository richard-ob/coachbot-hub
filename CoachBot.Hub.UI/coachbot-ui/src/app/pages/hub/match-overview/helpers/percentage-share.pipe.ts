import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'percentageSharePipe' })
export class PercentageSharePipe implements PipeTransform {
    transform(value: number, otherValues: number[]): string {
        const total = value + otherValues.reduce((a, b) => a + b, 0);

        return ((value / total) * 100).toFixed(2);
    }
}
