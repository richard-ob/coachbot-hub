import { Component } from '@angular/core';
import { Region } from '../model/region';
import { RegionService } from '../shared/services/region.service';

@Component({
    selector: 'app-regions',
    templateUrl: './regions.component.html'
})
export class RegionsComponent {

    newRegion: Region = new Region();
    regions: Region[];

    constructor(private regionService: RegionService) {
        this.regionService.getRegions().subscribe(regions => this.regions = regions);
    }

    addRegion() {
        this.regionService.addRegion(this.newRegion).subscribe(() => {
            this.regionService.getRegions().subscribe(regions => this.regions = regions);
            this.newRegion = new Region();
        });
    }
}
