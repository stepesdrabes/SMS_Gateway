import { Component, OnInit } from '@angular/core';
import { DataAccessService } from "../../services/data-access.service";

@Component({
  selector: 'app-devices',
  templateUrl: './devices.component.html',
  styleUrls: ['./devices.component.css']
})
export class DevicesComponent implements OnInit {

  constructor(public data: DataAccessService) {
  }

  ngOnInit(): void {

  }
}
