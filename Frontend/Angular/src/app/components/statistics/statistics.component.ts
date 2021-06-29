import { Component, OnInit } from '@angular/core';
import { DataAccessService } from "../../services/data-access.service";

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.css']
})
export class StatisticsComponent implements OnInit {

  constructor(public data: DataAccessService) {
  }

  ngOnInit(): void {

  }
}
