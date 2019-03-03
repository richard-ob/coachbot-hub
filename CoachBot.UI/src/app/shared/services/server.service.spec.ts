import { asyncData } from '../testing/async-observable-helper';
import { ServerService } from './server.service';
import { Server } from '../../model/server';

describe('ServerService', () => {
    let httpClientSpy: { get: jasmine.Spy };
    let serverService: ServerService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
        serverService = new ServerService(<any>httpClientSpy);
    });

    it('#getServers should return server list & call httpClient only once', () => {
        const expectedServers: Server[] = [
            { name: 'Server #1', address: '1.1.1.1:1000', regionId: 1, serverCount: 1 },
            { name: 'Server #2', address: '1.1.1.1:2000', regionId: 3, serverCount: 3 }
        ];
        httpClientSpy.get.and.returnValue(asyncData(expectedServers));

        serverService.getServers().subscribe(servers => expect(servers).toEqual(expectedServers, 'expected servers'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });
});
