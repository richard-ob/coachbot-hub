import { NgModule } from '@angular/core';
import { CalendarHeatmapComponent } from './calendar-heatmap.component';
import { NgxChartsModule } from '@swimlane/ngx-charts';

@NgModule({
    declarations: [
        CalendarHeatmapComponent
    ],
    exports: [
        CalendarHeatmapComponent
    ],
    imports: [
        NgxChartsModule
    ]
})
export class CalendarHeatmapModule { }
