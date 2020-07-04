import { Component, OnInit } from '@angular/core';
import { BotService } from '@pages/hub/shared/services/bot.service';

@Component({
    selector: 'app-bot-logs',
    templateUrl: './bot-logs.component.html'
})
export class BotLogsComponent implements OnInit {

    botLog: string;
    isRefreshing = false;
    isLoading = true;

    constructor(private botService: BotService) { }

    ngOnInit() {
        this.refreshLog();
    }

    refreshLog() {
        this.isRefreshing = true;
        this.botService.getBotLogs().subscribe(botLog => {
            this.botLog = botLog;
            this.isRefreshing = false;
            this.isLoading = false;
        });
    }
}
