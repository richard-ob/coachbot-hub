import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { PlayerService } from '@pages/hub/shared/services/player.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(private playerService: PlayerService, private router: Router) { }

    canActivate(): Observable<boolean> {
        return this.playerService.getCurrentPlayer().pipe(map(currentPlayer => {
            if (currentPlayer) {
                return true;
            }

            this.router.navigate(['/login']);
            return false;
        }));
    }

}
