import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CoreModule } from '@core/core.module';
import { CircleGraphComponent } from './circle-graph/circle-graph.component';
import { HorizontalBarGraphComponent } from './horizontal-bar-graph/horizontal-bar-graph.component';
import { PercentageSharePipe } from './percentage-share.pipe';
import { ThreeWayHorizontalBarGraphComponent } from './three-way-horizontal-bar-graph/three-way-horizontal-bar-graph.component';

@NgModule({
    declarations: [
        HorizontalBarGraphComponent,
        CircleGraphComponent,
        ThreeWayHorizontalBarGraphComponent,
        PercentageSharePipe
    ],
    exports: [
        HorizontalBarGraphComponent,
        CircleGraphComponent,
        ThreeWayHorizontalBarGraphComponent
    ],
    imports: [
        CommonModule,
        CoreModule
    ]
})
export class GraphModule { }
