import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { ConfigurationService } from './configuration.service';
import { EndpointBase } from './endpoint-base.service';
import { AuthService } from './auth.service';
import { ControllerRegistry } from '../models/controller-registry';

@Injectable({
  providedIn: 'root'
})
export class ControllerRegistryService extends EndpointBase {

    constructor(
        private configurations: ConfigurationService, http: HttpClient, authService: AuthService ) {
        super(http, authService);
    }

    public getAll() {
        const endpoint = this.configurations.baseUrl + this.configurations.controllerRegistryUrl;
        console.log(endpoint);
        return this.http.get<ControllerRegistry[]>(endpoint, this.requestHeaders);
    }

    public create(registry: ControllerRegistry) {
        const endpoint = this.configurations.baseUrl + this.configurations.controllerRegistryUrl;
        return this.http.post<ControllerRegistry[]>(endpoint, registry, this.requestHeaders);
    }

    public delete(id: string) {
        const endpoint = this.configurations.baseUrl + this.configurations.controllerRegistryUrl + `/${id}`;
        return this.http.delete(endpoint, this.requestHeaders);
    }

    public put(id: string, registry: ControllerRegistry) {
        const endpoint = this.configurations.baseUrl + this.configurations.controllerRegistryUrl + `/${id}`;
        return this.http.put(endpoint, registry, this.requestHeaders);
    }
}
