import { Component, ChangeDetectionStrategy } from '@angular/core';
import { Lightbox } from 'ngx-lightbox';

@Component({
    selector: 'app-media',
    templateUrl: './media.component.html',
    styleUrls: ['./media.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class MediaComponent {

    private albums = [];

    constructor(private lightbox: Lightbox) {
        for (let i = 1; i <= 12; i++) {
            const src = 'assets/images/iosoccer/screenshots/' + i + '.jpg';
            const caption = 'IOSoccer Screenshot';
            const thumb = 'assets/images/iosoccer/screenshots/' + i + '.jpg';
            const album = {
                src,
                caption,
                thumb
            };

            this.albums.push(album);
        }
    }

    open(index: number): void {
        this.lightbox.open(this.albums, index);
    }

    close(): void {
        this.lightbox.close();
    }

}
