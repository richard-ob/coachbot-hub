import { Component, OnInit } from '@angular/core';
import { BotService } from '@pages/hub/shared/services/bot.service';
import { Announcement } from '@pages/hub/shared/model/announcement.model';
import { Region } from '@pages/hub/shared/model/region.model';
import { RegionService } from '@pages/hub/shared/services/region.service';

@Component({
    selector: 'app-bot-announcements',
    templateUrl: './bot-announcements.component.html'
})
export class BotAnnouncementsComponent implements OnInit {

    regions: Region[];
    announcement = new Announcement();
    isSending = false;
    isLoading = true;

    constructor(private botService: BotService, private regionService: RegionService) { }

    ngOnInit() {
        this.regionService.getRegions().subscribe(regions => {
            this.regions = regions;
            this.isLoading = false;
        });
    }

    sendAnnouncement() {
        this.isSending = true;
        this.botService.sendAnnouncement(this.announcement).subscribe(() => {
            this.isSending = false;
            this.announcement = new Announcement();
        });
    }
}
