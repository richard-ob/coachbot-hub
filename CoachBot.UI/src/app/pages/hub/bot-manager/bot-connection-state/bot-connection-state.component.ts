import { Component, OnInit } from '@angular/core';
import { BotState } from '@pages/hub/shared/model/bot-state.model';
import { BotService } from '@pages/hub/shared/services/bot.service';

@Component({
    selector: 'app-bot-connection-state',
    templateUrl: './bot-connection-state.component.html'
})
export class BotConnectionStateComponent implements OnInit {

    botState: BotState;
    isReconnecting = false;
    isRefreshing = false;
    isLoading = true;

    constructor(private botService: BotService) { }

    ngOnInit() {
        this.refreshBotState();
        this.startAutomaticRefresh();
    }

    refreshBotState() {
        this.isRefreshing = true;
        this.botService.getBotState().subscribe(botState => {
            this.botState = botState;
            this.isLoading = false;
            this.isRefreshing = false;
        });
    }

    startAutomaticRefresh() {
        setInterval(() => this.refreshBotState(), 60000);
    }

    reconnectBot() {
        this.isReconnecting = true;
        this.botService.reconnectBot().subscribe(() => {
            this.botService.getBotState().subscribe(state => {
                this.botState = state;
                this.isReconnecting = false;
            });
        });
    }

    connectBot() {
        this.isReconnecting = true;
        this.botService.connectBot().subscribe(() => {
            this.botService.getBotState().subscribe(state => {
                this.botState = state;
                this.isReconnecting = false;
            });
        });
    }

    disconnectBot() {
        this.isReconnecting = true;
        this.botService.disconnectBot().subscribe(() => {
            this.botService.getBotState().subscribe(state => {
                this.botState = state;
                this.isReconnecting = false;
            });
        });
    }

}
