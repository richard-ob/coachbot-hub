import { Component, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { SteamService } from '@shared/services/steam.service.';

@Component({
    selector: 'app-news',
    templateUrl: './news.component.html'
})
export class NewsComponent implements OnInit {

    news: any[];

    constructor(private steamService: SteamService) { }

    ngOnInit(): void {
        this.steamService.getNews().subscribe(response => {
            for (const item of response.appnews.newsitems) {
                item.date = new Date(item.date * 1000);
                console.log(item.date);
            }
            this.news = response.appnews.newsitems;
        });
    }

    getFirstImage(content: string) {
        if (content.indexOf('[img]') > -1) {
            console.log(content);
            const image = content.split('[img]')[1].split('[/img]')[0];
            return image.replace('{STEAM_CLAN_IMAGE}', 'https://steamcdn-a.akamaihd.net/steamcommunity/public/images/clans/');
        }
        return 'https://iosoccer.com/assets/stadium-3.jpg';
    }

}
