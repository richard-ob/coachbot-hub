import { asyncData } from '../testing/async-observable-helper';
import { LogService } from './log.service';

describe('MatchService', () => {
    let httpClientSpy: { get: jasmine.Spy };
    let logService: LogService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
        logService = new LogService(<any>httpClientSpy);
    });

    it('#getLog should return a string & call httpClient only once', () => {
        const expectedLog = 'Log Entry Data';
        httpClientSpy.get.and.returnValue(asyncData(expectedLog));

        logService.getLog().subscribe(log => expect(log).toEqual(expectedLog, 'expected log'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });
});
