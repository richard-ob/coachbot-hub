import { Pipe, PipeTransform } from '@angular/core';
import { CountryNameConverter } from './country-list-converter';

@Pipe({ name: 'countryNameToCodeConverter' })
export class CountryNameConverterPipe implements PipeTransform {
    transform(countryName: string): string {
        return new CountryNameConverter().getCountryCode(countryName);
    }
}
