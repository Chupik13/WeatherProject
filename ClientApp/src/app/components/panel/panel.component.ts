import {Component, OnInit} from '@angular/core';
import {NzMessageService} from "ng-zorro-antd/message";
import {catchError, throwError} from "rxjs";
import {FilterOptions} from "../../models/FilterOptions";
import {ApiService} from "../../api.service";
import {TableDataService} from "../../table-data.service";

@Component({
  selector: 'app-panel',
  templateUrl: './panel.component.html',
  styleUrls: ['./panel.component.css']
})
export class PanelComponent implements OnInit {
  mode = 'year';
  dateValue: Date | null = null;

  constructor(
    private apiService: ApiService,
    private msg: NzMessageService,
    private tableDataService: TableDataService
  ) {
  }

  ngOnInit(): void {
  }

  onDateChange(): void {
    console.log('date change', this.dateValue);
    const filterOptions: FilterOptions = {
      month: null,
      year: null
    };
    if (this.dateValue) {
      filterOptions.year = this.dateValue.getFullYear();
      filterOptions.month = this.mode === 'month' ? this.dateValue.getMonth() + 1 : null;
    }
    this.tableDataService.updateFilterOptions(filterOptions);
  }

  clearWeatherArchive(): void {
    this.apiService.clearWeatherData()
      .pipe(
        catchError(err => {
          this.msg.error(err);
          return throwError(err);
        })
      )
      .subscribe(response => {
        this.msg.success('Данные успешно очищены!');
        this.tableDataService.refreshTable();
      });
  }
}

