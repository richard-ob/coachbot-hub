import { Component } from '@angular/core';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { AnnouncementService } from '../shared/services/announcement.service';
import { ChatMessage } from '../model/chat-message';

@Component({
    selector: 'app-announcements',
    templateUrl: './announcements.component.html'
})
export class AnnouncementsComponent {

    message = new ChatMessage();

    constructor(private route: ActivatedRoute, private announcementService: AnnouncementService) {
    }

    sendAnnouncement() {
        this.announcementService.sendAnnouncement(this.message).subscribe(complete => {
            this.message = new ChatMessage();
        });
    }
}
