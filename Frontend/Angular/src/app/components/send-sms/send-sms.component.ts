import {Component, OnInit} from '@angular/core';
import {HttpService} from "../../services/http.service";
import {CreateMessageModel} from "../../models";
import {NgForm} from "@angular/forms";
import {DataAccessService} from "../../services/data-access.service";

@Component({
  selector: 'app-send-sms',
  templateUrl: './send-sms.component.html',
  styleUrls: ['./send-sms.component.css']
})
export class SendSmsComponent implements OnInit {

  constructor(
    public data: DataAccessService,
    private http: HttpService) {
  }

  ngOnInit(): void {

  }

  onSubmit(form: NgForm) {
    const model: CreateMessageModel = {
      deviceId: null,
      recipient: form.value.prefix + form.value.phone,
      content: form.value.message,
      connectionId: this.data.signalR.connectionId
    };

    this.http.sendTextMessage(model)
      .subscribe(
        () => {
        },
        error => {
          console.log(error)
        });

    form.reset();

    this.data.sendingMessage = true;
  }
}
