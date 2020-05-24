import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SpinnerModule } from 'src/app/core/components/spinner/spinner.module';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { FormIndicatorModule } from '../shared/components/form-indictator/form-indicator.module';
import { ThSorterModule } from '@shared/components/th-sorter/th-sort.module';
import { TeamEditorListComponent } from './team-editor-list.component';
import { NgPipesModule } from 'ngx-pipes';
import { SweetAlert2Module } from '@sweetalert2/ngx-sweetalert2';
import { HubPipesModule } from '../shared/pipes/hub-pipes.module';

@NgModule({
    declarations: [
        TeamEditorListComponent
    ],
    exports: [
        TeamEditorListComponent
    ],
    imports: [
        CommonModule,
        RouterModule,
        FormsModule,
        SpinnerModule,
        NgxPaginationModule,
        FormIndicatorModule,
        ThSorterModule,
        NgPipesModule,
        SweetAlert2Module,
        HubPipesModule
    ]
})
export class TeamEditorListModule { }
