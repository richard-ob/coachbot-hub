import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RequestOptionsInterceptor } from './interceptors/request-options.interceptor';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { UnauthorizedInterceptor } from './interceptors/unauthorized.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';

@NgModule({
    imports: [
        CommonModule
    ],
    providers: [
        {
            provide: HTTP_INTERCEPTORS,
            useClass: RequestOptionsInterceptor,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: UnauthorizedInterceptor,
            multi: true
        },
        {
            provide: HTTP_INTERCEPTORS,
            useClass: ErrorInterceptor,
            multi: true
        }
    ]
})
export class CoreModule { }
