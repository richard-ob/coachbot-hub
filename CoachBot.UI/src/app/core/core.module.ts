import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RequestOptionsInterceptor } from './interceptors/request-options.interceptor';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { UnauthorizedInterceptor } from './interceptors/unauthorized.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { TabNavMobileComponent } from './components/tab-nav-mobile/tab-nav-mobile.component';
import { AuthGuard } from './guards/auth.guard';

@NgModule({
    imports: [
        CommonModule
    ],
    providers: [
        AuthGuard,
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
    ],
    declarations: [
        TabNavMobileComponent
    ],
    exports: [
        TabNavMobileComponent
    ]
})
export class CoreModule { }
