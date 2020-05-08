import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CounterinfoComponent } from './counterinfo.component';

describe('CounterinfoComponent', () => {
  let component: CounterinfoComponent;
  let fixture: ComponentFixture<CounterinfoComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CounterinfoComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CounterinfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
