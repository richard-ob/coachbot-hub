import { Component, OnInit, Input } from '@angular/core';

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
  @Input() awayValue: number;
  @Input() awayColour = '#38a9ff';
  @Input() title: string;
  @Input() displayValueMode: DisplayValueMode = DisplayValueMode.Value;
  displayValueModes = DisplayValueMode;

  constructor() { }

  ngOnInit() {

  }

}
