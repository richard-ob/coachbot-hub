import { Component } from '@angular/core';
import { UserService } from './core/services/user.service';
import { User } from './core/models/user.model';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {

  user: User;
  apiUrl = environment.apiUrl;

  constructor(private userService: UserService) {
    this.userService.getUser().subscribe(user => this.user = user);
  }

}
