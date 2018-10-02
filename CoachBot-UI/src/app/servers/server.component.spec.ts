import { TestBed, async, ComponentFixture, ComponentFixtureAutoDetect, } from '@angular/core/testing';
import { ServersComponent } from './servers.component';
import { ServerService } from '../shared/services/server.service';
import { RegionService } from '../shared/services/region.service';
import { FormsModule } from '@angular/forms';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { of } from 'rxjs';

describe('ServerComponent', () => {

    let component: ServersComponent;
    let fixture: ComponentFixture<ServersComponent>;
    let getServersSpy: { getServers: jasmine.Spy };
    let getRegionsSpy: { getRegions: jasmine.Spy };
    beforeEach(() => {

        const testServers = [
            { serverName: 'Test Server #1', address: '1.1.1.1:1000', serverCount: 3 },
            { serverName: 'Test Server #2', address: '1.1.1.1:2000', serverCount: 3 }
        ];
        const testRegions = [{ regionName: 'Europe' }, { regionName: 'South America' }, { regionName: 'North America' }];

        const serverService = jasmine.createSpyObj('ServerService', ['getServers']);
        const regionService = jasmine.createSpyObj('RegionService', ['getRegions']);

        getServersSpy = serverService.getServers.and.returnValue(of(testServers));
        getRegionsSpy = regionService.getRegions.and.returnValue(of(testRegions));

        TestBed.configureTestingModule({
            declarations: [ServersComponent],
            providers: [
                { provide: ServerService, useValue: serverService },
                { provide: RegionService, useValue: regionService },
                { provide: ComponentFixtureAutoDetect, useValue: true }
            ],
            imports: [
                FormsModule,
                HttpClientTestingModule
            ]
        });

        fixture = TestBed.createComponent(ServersComponent);
        component = fixture.componentInstance;
    });

    it('should create the servers component', async(() => {
        expect(component).toBeTruthy();
    }));

    it('should display table of servers from mocked data passed in', async(() => {
        const tbody = fixture.nativeElement.querySelector('table tbody');
        expect(tbody.childElementCount).toEqual(2);
    }));

    it('should display select list of regions from mocked data passed in', async(() => {
        const tbody = fixture.nativeElement.querySelector('#regionId');
        expect(tbody.childElementCount).toEqual(3);
    }));
});
