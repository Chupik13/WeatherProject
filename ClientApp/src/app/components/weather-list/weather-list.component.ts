import {Component, ElementRef, OnInit, TemplateRef, ViewChild} from '@angular/core';
import {WeatherUnit, WindDirection} from "../../models/WeatherUnit";
import {FilterOptions} from "../../models/FilterOptions";
import {NzTableQueryParams} from "ng-zorro-antd/table";
import {NzUploadChangeParam, NzUploadFile} from "ng-zorro-antd/upload";
import {NzMessageService} from "ng-zorro-antd/message";
import {NzConfigService} from 'ng-zorro-antd/core/config';
import {TableDataService} from "../../table-data.service";
import {ApiService} from "../../api.service";

@Component({
  selector: 'app-weather-list',
  templateUrl: './weather-list.component.html',
  styleUrls: ['./weather-list.component.css']
})
export class WeatherListComponent implements OnInit {
  sendWeatherArchiveUrl?: string;
  isDragOver = false;
  total = 1;
  listOfWeatherData: WeatherUnit[] = [];
  loading = true;
  pageSize = 20;
  pageIndex = 1;
  filterOptions?: FilterOptions;
  @ViewChild('customTpl', {static: false}) customTpl?: TemplateRef<any>;

  constructor(
    private apiService: ApiService,
    private msg: NzMessageService,
    private tableDataService: TableDataService,
    private nzConfigService: NzConfigService,
    private elementRef: ElementRef
  ) {
    this.sendWeatherArchiveUrl = apiService.sendWeatherArchiveUrl();
  }

  ngOnInit(): void {
    this.refreshTable(this.pageIndex, this.pageSize);
    this.tableDataService.refreshTable$.subscribe(() => {
      this.refreshTable(1, this.pageSize, this.filterOptions);
    });
    this.tableDataService.updateFilterOptions$.subscribe(filterOptions => {
      this.filterOptions = filterOptions;
      this.refreshTable(1, this.pageSize, this.filterOptions);
    });
  }

  refreshTable(pageIndex: number, pageSize: number, filterOptions?: FilterOptions): void {
    this.nzConfigService.set('empty', {nzDefaultEmptyContent: this.customTpl});
    this.loading = true;
    this.apiService.getTotalWeatherArchive(filterOptions).subscribe(data => {
      this.total = data;
    }, error => this.handleError(error));
    this.apiService.getWeatherArchive(pageIndex, pageSize, filterOptions).subscribe(data => {
      this.loading = false;
      this.listOfWeatherData = data;
      this.pageIndex = pageIndex;
    }, error => this.handleError(error));
  }

  handleError(error: any): void {
    this.msg.error(error);
    this.loading = false;
  }

  onPageIndexChange(params: NzTableQueryParams): void {
    const {pageSize, pageIndex} = params;
    this.refreshTable(pageIndex, pageSize, this.filterOptions);
  }

  windDirectionFormat(windDirections: WindDirection[]): string {
    return windDirections.map(direction => direction.name).join(', ');
  }

  handleChange(info: NzUploadChangeParam): void {
    if (info.file.status === 'done') {
      this.msg.success(`Файл успешно загружен`);
      this.refreshTable(this.pageIndex, this.pageSize, this.filterOptions);
    } else if (info.file.status === 'error') {
      this.msg.error(`${info.file.name} Ошибка загрузки.  Файл некорректен, либо содержит уже имеющиеся данные`);
    }
  }

  handleDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragOver = true;
  }

  handleUselesDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
  }

  handleUselesDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
  }

  handleDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    if (!this.elementRef.nativeElement.contains(event.relatedTarget)) {
      this.isDragOver = false;
    }
  }

  beforeUpload = (file: NzUploadFile, files: NzUploadFile[]): boolean => {
    const ext = file.name.split('.').pop();
    if (ext !== 'xls' && ext !== 'xlsx') {
      this.msg.error(`Файл должен иметь расширение xls или xlsx`);
      return false;
    }
    return true;
  }
}
