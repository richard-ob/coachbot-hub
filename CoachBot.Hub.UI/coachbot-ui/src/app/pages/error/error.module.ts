import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ErrorComponent } from './error.component';

@NgModule({
    declarations: [
        ErrorComponent
    ],
    imports: [
        CommonModule,
        FormsModule
    ]
})
export class ErrorModule { }
