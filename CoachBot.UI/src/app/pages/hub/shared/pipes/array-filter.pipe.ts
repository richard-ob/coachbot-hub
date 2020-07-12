import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'arrayFilter', pure: false })
export class ArrayFilterPipe implements PipeTransform {
    transform(
        baseArray: any[], comparisonProperty: string, comparisonValue: string, mode: ArrayFilterPipeMode = ArrayFilterPipeMode.Equals
    ): any[] {
        if (mode === ArrayFilterPipeMode.NotEquals) {
            return baseArray.filter(b => b[comparisonProperty] !== comparisonValue);
        }
        return baseArray.filter(b => b[comparisonProperty] === comparisonValue);
    }
}

export enum ArrayFilterPipeMode {
    Equals,
    NotEquals
}
