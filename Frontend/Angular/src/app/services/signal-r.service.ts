import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { environment } from "../../environments/environment";

@Injectable({
  providedIn: 'root'
})
export class SignalRService {

  public hubConnection: signalR.HubConnection = new signalR.HubConnectionBuilder().withUrl(environment.HUB_URL).build();

  public connectionId: string | undefined;

  constructor() {
  }

  public connect() {
    this.hubConnection
      .start()
      .then(() => console.log('Connection to SignalR server started'))
      .catch(error => console.log('Error while starting connection: ' + error));

    this.registerConnectionIdListener();
  }

  registerConnectionIdListener() {
    this.hubConnection.on('ConnectionId', (connectionId) => {
      this.connectionId = connectionId;
      console.log('ConnectionID:' + connectionId)
    });
  }
}
