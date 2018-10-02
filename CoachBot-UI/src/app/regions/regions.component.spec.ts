import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect, } from '@angular/core/testing';
import { RegionsComponent } from './regions.component';
import { RegionService } from '../shared/services/region.service';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';
import { Region } from '../model/region';

describe('RegionsComponent', () => {

    let component: RegionsComponent;
    let fixture: ComponentFixture<RegionsComponent>;
    let getRegionsSpy: { getRegions: jasmine.Spy };
    beforeEach(() => {

        const testRegions: Region[] = [
            { regionId: 1, regionName: 'Europe', serverCount: 3 },
            { regionId: 2, regionName: 'South America', serverCount: 0 },
            { regionId: 3, regionName: 'North America', serverCount: 0 }
        ];
        const regionService = jasmine.createSpyObj('RegionService', ['getRegions']);

        getRegionsSpy = regionService.getRegions.and.returnValue(of(testRegions));

        TestBed.configureTestingModule({
            declarations: [RegionsComponent],
            providers: [
                { provide: RegionService, useValue: regionService },
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ],
            imports: [
                FormsModule,
                HttpClientTestingModule
            ]
        });

        fixture = TestBed.createComponent(RegionsComponent);
        component = fixture.componentInstance;
    });

    it('should create the regions component', async(() => {
        expect(component).toBeTruthy();
    }));

    it('should display table of regions from mocked data passed in', async(() => {
        const tbody = fixture.nativeElement.querySelector('table tbody');
        expect(tbody.childElementCount).toEqual(3);
    }));

    it('should disable remove region button when region has serverCount greater than 1', async(() => {
        const buttons = fixture.nativeElement.querySelectorAll('td button');
        expect(buttons[0].disabled).toEqual(true);
    }));

    it('should enable remove region button when region has serverCount equal to 0', async(() => {
        const buttons = fixture.nativeElement.querySelectorAll('td button');
        expect(buttons[1].disabled).toEqual(false);
    }));
});
