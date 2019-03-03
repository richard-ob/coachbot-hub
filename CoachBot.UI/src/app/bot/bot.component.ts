import { Component } from '@angular/core';
import { BotState } from '../model/bot-state';
import { BotService } from '../shared/services/bot.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { Configuration } from '../model/configuration';
import { LogService } from '../shared/services/log.service';

@Component({
    selector: 'app-bot',
    templateUrl: './bot.component.html'
})
export class BotComponent {

    botState: BotState;
    config: Configuration;
    log: string;
    refreshingLog = false;

    constructor(private botService: BotService, private configurationService: ConfigurationService, private logService: LogService) {
        this.botService.getBotState().subscribe(botState => {
            this.botState = botState;
            this.configurationService.getConfiguration().subscribe(config => this.config = config);
            this.logService.getLog().subscribe(log => this.log = log);
        });
        setTimeout(t => this.botService.getBotState().subscribe(state => this.botState = state), 10000);
    }

    saveConfig() {
        this.configurationService.updateConfiguration(this.config).subscribe();
    }

    reconnectBot() {
        this.botService.reconnectBot().subscribe(complete => {
            this.botService.getBotState().subscribe(state => this.botState = state);
        });
    }

    leaveGuild(id: number) {
        this.botService.leaveGuild(id).subscribe();
    }

    refreshLog() {
        this.refreshingLog = true;
        this.logService.getLog().subscribe(log => { this.log = log; this.refreshingLog = false; });
    }
}
