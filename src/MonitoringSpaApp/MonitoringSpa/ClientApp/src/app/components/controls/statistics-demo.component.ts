import { Component, OnInit, OnDestroy } from '@angular/core';
import { AlertService, DialogType, MessageSeverity } from '../../services/alert.service';
import { Subscription, Observable, fromEvent, of, merge } from 'rxjs';
import { map, distinctUntilChanged } from 'rxjs/operators';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../../environments/environment.prod';
import { AuthService } from '../../services/auth.service';

require('chart.js');

@Component({
  selector: 'statistics-demo',
  templateUrl: './statistics-demo.component.html',
  styleUrls: ['./statistics-demo.component.scss']
})
export class StatisticsDemoComponent implements OnInit, OnDestroy {

  chartData = [
    { data: [], label: 'Please wait until data is loaded' },
  ];
    timeclause = "lastweek";

  //chartLabels = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
  chartLabels = [];
  chartOptions = {
    responsive: true,
    title: {
      display: false,
      fontSize: 16,
      text: 'Counter data'
    }
  };
  chartColors = [
    { // grey
      backgroundColor: 'rgba(148,159,177,0.2)',
      borderColor: 'rgba(148,159,177,1)',
      pointBackgroundColor: 'rgba(148,159,177,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(148,159,177,0.8)'
    },
    { // dark grey
      backgroundColor: 'rgba(77,83,96,0.2)',
      borderColor: 'rgba(77,83,96,1)',
      pointBackgroundColor: 'rgba(77,83,96,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(77,83,96,1)'
    },
    { // something else
      backgroundColor: 'rgba(128,128,128,0.2)',
      borderColor: 'rgba(128,128,128,1)',
      pointBackgroundColor: 'rgba(128,128,128,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(128,128,128,0.8)'
    }
  ];
  chartLegend = true;
  chartType = 'line';

  timerReference: any;
  windowWidth$: Observable<number>;
  windowWidthSub: Subscription;


    constructor(private alertService: AlertService, private http: HttpClient, private authService: AuthService) {
  }


    protected get requestHeaders(): { headers: HttpHeaders | { [header: string]: string | string[]; } } {
        const headers = new HttpHeaders({
            Authorization: 'Bearer ' + this.authService.accessToken,
            'Content-Type': 'application/json',
            Accept: 'application/json, text/plain, */*'
        });

        return { headers };
    }

    ngOnInit() {
        this.refresh();
    const initialWidth$ = of(window.innerWidth);
    const resizedWidth$ = fromEvent(window, 'resize').pipe(map((event: any) => event.target.innerWidth as number));
    this.windowWidth$ = merge(initialWidth$, resizedWidth$).pipe(distinctUntilChanged());

    this.windowWidthSub = this.windowWidth$.subscribe(width => this.chartLegend = width < 600 ? false : true);
  }

  ngOnDestroy() {
    clearInterval(this.timerReference);
    this.windowWidthSub.unsubscribe();
  }

    refresh(): void {
        const url = `${environment.dashboardDatUrl}/${this.timeclause}`;
        this.http
            .get<any>(url, this.requestHeaders)
            .subscribe(
                res => {
                    console.log(res);
                    this.chartData = [];
                    this.chartData.push({ data: res.chartData, label: res.chartTitle });
                    this.chartLabels = res.chartLabels;
                    console.log(this.chartData);
                },
                error => {
                    this.alertService.showMessage('Error', 'Unable to refresh', MessageSeverity.error);
                });
    }

  randomize(): void {
    const _chartData = new Array(this.chartData.length);
    for (let i = 0; i < this.chartData.length; i++) {
      _chartData[i] = { data: new Array(this.chartData[i].data.length), label: this.chartData[i].label };

      for (let j = 0; j < this.chartData[i].data.length; j++) {
        _chartData[i].data[j] = Math.floor((Math.random() * 100) + 1);
      }
    }

    this.chartData = _chartData;
  }

  changeChartType(type: string) {
    this.chartType = type;
    }

    changeTimeClause(time: string) {
        this.timeclause = time;
        this.refresh();
    }

  showMessage(msg: string): void {
    this.alertService.showMessage('Info', msg, MessageSeverity.info);
  }

  showDialog(msg: string): void {
    this.alertService.showDialog(msg, DialogType.prompt, (val) => this.configure(true, val), () => this.configure(false));
  }

  configure(response: boolean, value?: string) {

    if (response) {

      this.alertService.showStickyMessage('Simulating...', '', MessageSeverity.wait);

      setTimeout(() => {
        this.alertService.resetStickyMessage();
        this.alertService.showMessage('Demo', `Your settings was successfully configured to \"${value}\"`, MessageSeverity.success);
      }, 2000);
    } else {
      this.alertService.showMessage('Demo', 'Operation cancelled by user', MessageSeverity.default);
    }
  }

  chartClicked(e): void {
    console.log(e);
  }

  chartHovered(e): void {
    console.log(e);
  }
}
