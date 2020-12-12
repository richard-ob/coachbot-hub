import { Component, OnInit, Input } from '@angular/core';
import ColourUtils from '@shared/utilities/colour-utilities';
import { DisplayValueMode } from '../horizontal-bar-graph/horizontal-bar-graph.component';

@Component({
  selector: 'app-three-way-horizontal-bar-graph',
  templateUrl: './three-way-horizontal-bar-graph.component.html',
  styleUrls: ['./three-way-horizontal-bar-graph.component.scss']
})
export class ThreeWayHorizontalBarGraphComponent implements OnInit {

  @Input() valueOne: number;
  @Input() colourOne = '#dc3545';
  @Input() fontColourOne: string;
  @Input() valueTwo: number;
  @Input() colourTwo = '#38a9ff';
  @Input() fontColourTwo: string;
  @Input() valueThree: number;
  @Input() colourThree = '#38a9ff';
  @Input() fontColourThree: string;
  @Input() title: string;
  @Input() displayValueMode: DisplayValueMode = DisplayValueMode.Value;
  displayValueModes = DisplayValueMode;

  constructor() { }

  ngOnInit() {
    this.fontColourOne = this.getFontColour(this.colourOne);
    this.fontColourTwo = this.getFontColour(this.colourTwo);
    this.fontColourThree = this.getFontColour(this.colourThree);
  }

  getFontColour(color): string {
    return ColourUtils.isDark(color) ? '#000000' : '#ffffff';
  }


}
