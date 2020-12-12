import { Component, OnInit, Input } from '@angular/core';
import ColourUtils from '@shared/utilities/colour-utilities';

export enum DisplayValueMode {
  Value,
  Percentage
}

@Component({
  selector: 'app-horizontal-bar-graph',
  templateUrl: './horizontal-bar-graph.component.html',
  styleUrls: ['./horizontal-bar-graph.component.scss']
})
export class HorizontalBarGraphComponent implements OnInit {

  @Input() homeValue: number;
  @Input() homeColour = '#dc3545';
  @Input() homeColourFont: string;
  @Input() awayValue: number;
  @Input() awayColour = '#38a9ff';
  @Input() awayColourFont: string;
  @Input() title: string;
  @Input() displayValueMode: DisplayValueMode = DisplayValueMode.Value;
  displayValueModes = DisplayValueMode;

  constructor() { }

  ngOnInit() {
    this.homeColourFont = this.getFontColour(this.homeColour);
    this.awayColourFont = this.getFontColour(this.awayColour);
  }

  getFontColour(color): string {
    return ColourUtils.isDark(color) ? '#000000' : '#ffffff';
  }


}
