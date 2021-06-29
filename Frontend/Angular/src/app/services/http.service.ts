import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { Observable, of } from "rxjs";
import { CreateMessageModel, Device, ServiceStatistics } from "../models";
import { environment } from "../../environments/environment";
import { catchError, tap } from "rxjs/operators";

@Injectable({
  providedIn: 'root'
})
export class HttpService {

  constructor(private http: HttpClient) {
  }

  getServiceStatistics(): Observable<ServiceStatistics> {
    return this.http.get<ServiceStatistics>(`${environment.BASE_URL}/service/statistics`);
  }

  getDevices(): Observable<Array<Device>> {
    return this.http.get<Array<Device>>(`${environment.BASE_URL}/devices`);
  }

  sendTextMessage(model: CreateMessageModel) {
    return this.http.post<ServiceStatistics>(`${environment.BASE_URL}/messages`, {
      deviceId: model.deviceId,
      recipient: model.recipient,
      content: model.content,
      connectionId: model.connectionId
    }).pipe(
      tap(_ => console.log('Message sent to server')),
      catchError(error => of([]))
    );
  }
}
