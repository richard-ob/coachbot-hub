import { Component } from '@angular/core';
import { environment } from '../../environments/environment';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html'
})
export class LoginComponent {

    apiUrl = environment.apiUrl;

    constructor() {
    }

}
