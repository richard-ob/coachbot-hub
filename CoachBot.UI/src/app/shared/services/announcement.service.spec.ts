import { AnnouncementService } from './announcement.service';
import { ChatMessage } from '../../model/chat-message';
import { asyncData } from '../testing/async-observable-helper';

describe('AnnouncementService', () => {
    let httpClientSpy: { post: jasmine.Spy };
    let announcementService: AnnouncementService;

    beforeEach(() => {
        httpClientSpy = jasmine.createSpyObj('HttpClient', ['post']);
        announcementService = new AnnouncementService(<any>httpClientSpy);
    });

    it('#sendAnnouncement should post announcement & httpClient only once', () => {
        const chatMessage: ChatMessage = { message: 'test message', sender: 'Test User' };
        httpClientSpy.post.and.returnValue(asyncData(true));

        announcementService.sendAnnouncement(chatMessage).subscribe();

        expect(httpClientSpy.post.calls.count()).toBe(1, 'one call');
    });
});
