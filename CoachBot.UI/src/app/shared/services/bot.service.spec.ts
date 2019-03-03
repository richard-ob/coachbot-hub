import { asyncData } from '../testing/async-observable-helper';
import { BotService } from './bot.service';
import { BotState } from '../../model/bot-state';

describe('BotService', () => {
    let httpClientSpy: { get: jasmine.Spy };
    let botService: BotService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['get']);
        botService = new BotService(<any>httpClientSpy);
    });

    it('#getBotState should return region list & call httpClient only once', () => {
        const expectedBotState: BotState = { connectionStatus: 'Connected', loginStatus: 'Logged In', guilds: [] };
        httpClientSpy.get.and.returnValue(asyncData(expectedBotState));

        botService.getBotState().subscribe(botState => expect(botState).toEqual(expectedBotState, 'expected bot state'), fail);

        expect(httpClientSpy.get.calls.count()).toBe(1, 'one call');
    });
});
