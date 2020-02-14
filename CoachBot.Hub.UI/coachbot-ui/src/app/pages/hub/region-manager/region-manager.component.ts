import { Component } from '@angular/core';
import { Region } from '../shared/model/region.model';
import { RegionService } from '../shared/services/region.service';

@Component({
    selector: 'app-region-manager',
    templateUrl: './region-manager.component.html'
})
export class RegionManagerComponent {

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

    removeRegion(regionId: number) {
        this.regionService.removeRegion(regionId).subscribe(() => {
            this.regionService.getRegions().subscribe(regions => this.regions = regions);
        });
    }
}
