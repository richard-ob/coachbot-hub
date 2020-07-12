import { Component, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { SteamService } from '@shared/services/steam.service.';

@Component({
    selector: 'app-news',
    templateUrl: './news.component.html',
    styleUrls: ['./news.component.scss']
})
export class NewsComponent implements OnInit {

    news: any[];
    isLoading = true;

    constructor(private steamService: SteamService) { }

    ngOnInit(): void {
        this.steamService.getNews().subscribe(response => {
            for (const item of response.appnews.newsitems) {
                item.date = new Date(item.date * 1000);
            }
            this.news = response.appnews.newsitems;
            for (const newsItem of this.news) {
                newsItem.summary = this.getContentSummary(newsItem.contents);
                newsItem.image = this.getFirstImage(newsItem.contents);
            }
            this.isLoading = false;
        });
    }

    getFirstImage(content: string) {
        if (content.indexOf('[img]') > -1) {
            const image = content.split('[img]')[1].split('[/img]')[0];
            return image.replace('{STEAM_CLAN_IMAGE}', 'https://steamcdn-a.akamaihd.net/steamcommunity/public/images/clans/');
        }
        return 'https://iosoccer.com/assets/stadium-3.jpg';
    }

    getContentSummary(content: string) {
        let readableContent = content;
        readableContent = readableContent
            .replace(/\[.*?\]/g, ' ') // Steam markup
            .replace(/(?:https?|ftp):\/\/[\n\S]+/g, '') // links
            .replace(/^(.+)\/([^/]+)/g, ''); // file paths

        return readableContent.trim();
    }

}
