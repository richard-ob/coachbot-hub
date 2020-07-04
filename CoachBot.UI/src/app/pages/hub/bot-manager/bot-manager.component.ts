import { Component, OnInit } from '@angular/core';
import { BotService } from '../shared/services/bot.service';

@Component({
    selector: 'app-bot-manager',
    templateUrl: './bot-manager.component.html'
})
export class BotManagerComponent implements OnInit {

    constructor(private botService: BotService) { }

    ngOnInit() { }

}
