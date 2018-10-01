import { asyncData } from '../testing/async-observable-helper';
import { RegionService } from './region.service';
import { Region } from '../../model/region';

describe('RegionService', () => {
    let httpClientSpy: { get: jasmine.Spy };
    let regionService: RegionService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
        regionService = new RegionService(<any>httpClientSpy);
    });

    it('#getRegions should return region list & call httpClient only once', () => {
        const expectedRegions: Region[] = [
            { regionId: 1, regionName: 'Europe', serverCount: 3 },
            { regionId: 2, regionName: 'North America', serverCount: 3 }
        ];
        httpClientSpy.get.and.returnValue(asyncData(expectedRegions));

        regionService.getRegions().subscribe(regions => expect(regions).toEqual(expectedRegions, 'expected regions'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });
});
