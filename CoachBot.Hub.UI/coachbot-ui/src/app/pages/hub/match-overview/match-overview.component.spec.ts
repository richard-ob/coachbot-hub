import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MatchOverviewComponent } from './match-overview.component';

describe('MatchOverviewComponent', () => {
  let component: MatchOverviewComponent;
  let fixture: ComponentFixture<MatchOverviewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [MatchOverviewComponent]
    })
      .compileComponents();
    fixture = TestBed.createComponent(MatchOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }));

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
