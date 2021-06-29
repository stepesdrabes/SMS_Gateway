import { Injectable } from '@angular/core';
import { SignalRService } from "./signal-r.service";
import { HttpService } from "./http.service";
import { Device, ServiceStatistics } from "../models";

@Injectable({
  providedIn: 'root'
})
export class DataAccessService {

  public devices: Array<Device> | undefined;

  public statistics: ServiceStatistics | undefined;

  public sendingMessage: boolean = false;
  public state: number | null = null;

  constructor(public signalR: SignalRService, private http: HttpService) {
    this.registerStatisticsChangeListener();
    this.registerDevicesChangeListener();
    this.registerMessageDelivered();

    this.getStatistics();
    this.getDevices();

    this.signalR.connect();
  }

  getStatistics() {
    this.http
      .getServiceStatistics()
      .subscribe((serviceStatistics: ServiceStatistics) => {
        this.statistics = serviceStatistics;
      });
  }

  getDevices() {
    this.http
      .getDevices()
      .subscribe((devices: Array<Device>) => {
        this.devices = devices;
      });
  }

  registerStatisticsChangeListener() {
    this.signalR.hubConnection.on('StatisticsChange', () => {
      this.getStatistics();
    })
  }

  registerDevicesChangeListener() {
    this.signalR.hubConnection.on('DevicesChange', () => {
      this.getDevices();
    });
  }

  registerMessageDelivered() {
    this.signalR.hubConnection.on('MessageSent', (state) => {
      this.state = state;

      setTimeout(() => {
        this.state = null;
        this.sendingMessage = false;
      }, 3000);
    });
  }
}
