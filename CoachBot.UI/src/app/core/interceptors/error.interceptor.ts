import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router, ActivatedRoute } from '@angular/router';
declare var window: any;

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
    constructor(private router: Router, private activatedRoute: ActivatedRoute) { }

    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(request).pipe(catchError(err => {
            if (err instanceof HttpErrorResponse && window.location.href.indexOf('login') === -1) {
                if ((err.status === 500 || err.status === 0) && this.router.url.length > 1) {
                    this.router.navigate(['/error'], { skipLocationChange: true });
                }
            }
            return throwError(err);
        }));
    }
}
