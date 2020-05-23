import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThSorterComponent } from './th-sorter.component';

@NgModule({
    declarations: [
        ThSorterComponent
    ],
    exports: [
        ThSorterComponent
    ],
    imports: [
        CommonModule
    ]
})
export class ThSorterModule { }
