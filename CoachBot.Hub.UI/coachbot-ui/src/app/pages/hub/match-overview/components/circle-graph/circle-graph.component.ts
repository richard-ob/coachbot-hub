import { Component, OnInit, Input } from '@angular/core';


@Component({
  selector: 'app-circle-graph',
  templateUrl: './circle-graph.component.html',
  styleUrls: ['./circle-graph.component.scss']
})
export class CircleGraphComponent implements OnInit {

  @Input() totalValue: number;
  @Input() partValue: number;
  @Input() title: string;
  @Input() fillColour: string;

  constructor() { }

  ngOnInit() {
  }

}
