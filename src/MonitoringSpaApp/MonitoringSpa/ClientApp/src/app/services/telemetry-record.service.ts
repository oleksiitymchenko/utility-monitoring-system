import { Injectable } from '@angular/core';
import { EndpointBase } from './endpoint-base.service';
import { ConfigurationService } from './configuration.service';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { TelemetryRecord } from '../models/telemetry-record';

@Injectable({
  providedIn: 'root'
})
export class TelemetryRecordService extends EndpointBase {

    constructor(
        private configurations: ConfigurationService, http: HttpClient, authService: AuthService) {
        super(http, authService);
    }

    public getAll() {
        const endpoint = this.configurations.baseUrl + this.configurations.telemetryDataUrl;
        console.log(endpoint);
        return this.http.get<TelemetryRecord[]>(endpoint, this.requestHeaders);
    }

    public create(registry: TelemetryRecord) {
        const endpoint = this.configurations.baseUrl + this.configurations.telemetryDataUrl;
        return this.http.post<TelemetryRecord[]>(endpoint, registry, this.requestHeaders);
    }

    public delete(id: string) {
        const endpoint = this.configurations.baseUrl + this.configurations.telemetryDataUrl + `/${id}`;
        return this.http.delete(endpoint, this.requestHeaders);
    }

    public put(id: string, registry: TelemetryRecord) {
        const endpoint = this.configurations.baseUrl + this.configurations.telemetryDataUrl + `/${id}`;
        return this.http.put(endpoint, registry, this.requestHeaders);
    }
}
