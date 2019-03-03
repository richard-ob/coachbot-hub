import { ConfigurationService } from './configuration.service';
import { Configuration } from '../../model/configuration';
import { asyncData } from '../testing/async-observable-helper';

describe('ConfigurationService', () => {
    let httpClientSpy: { get: jasmine.Spy };
    let configurationService: ConfigurationService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
        configurationService = new ConfigurationService(<any>httpClientSpy);
    });

    it('#getConfiguration should return array of players & call httpClient only once', () => {
        const expectedConfiguration: Configuration = { botToken: 'fadsj£$#_not_real_fdNCV3adsfka' };
        httpClientSpy.get.and.returnValue(asyncData(expectedConfiguration));

        configurationService.getConfiguration()
            .subscribe(configuration => expect(configuration).toEqual(expectedConfiguration, 'expected configuration'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });
});
